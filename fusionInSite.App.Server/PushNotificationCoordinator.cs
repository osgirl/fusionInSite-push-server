﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fusionInsiteServicesData.Cache;
using FusionInsite.App.Server.Data;
using FusionInsite.App.Server.Data.Models;
using FusionInsite.App.Server.Data.Repositories;
using FusionInsite.App.Server.Data.Repositories.Interfaces;
using FusionInsite.App.Server.GetNewNotifications;
using FusionInsite.App.Server.PushNotificationSender;
using log4net;

namespace FusionInsite.App.Server
{
    public interface IPushNotificationCoordinator
    {
        /// <summary>
        /// Get All New Notifications
        /// -> Filter by already sent
        /// -> Generate one message for each subscribed user
        /// -> Group multiple messages for each user
        /// -> Get single message text per user
        /// -> Group the identical messages
        /// -> Send each different message to a list of user tokens
        /// </summary>
        void Send();
    }

    public class PushNotificationCoordinator : IPushNotificationCoordinator
    {
        private readonly ILog _log;
        private readonly INotificationHistoryRepository _notificationHistoryRepository;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly IEnumerable<IGetNewNotifications> _notificationMakers;
        private readonly IPushNotificationSender _pushNotificationSender;

        public PushNotificationCoordinator(ILog log, INotificationHistoryRepository notificationHistoryRepository, IUserSubscriptionRepository userSubscriptionRepository, IEnumerable<IGetNewNotifications> notificationMakers, IPushNotificationSender pushNotificationSender) // Ninject will bind all instances
        {
            _log = log;
            _notificationHistoryRepository = notificationHistoryRepository;
            _userSubscriptionRepository = userSubscriptionRepository;
            _notificationMakers = notificationMakers;
            _pushNotificationSender = pushNotificationSender;
        }

        /// <summary>
        /// Get All New Notifications
        /// -> Filter by already sent
        /// -> Generate one message for each subscribed user
        /// -> Group multiple messages for each user
        /// -> Get single message text per user
        /// -> Group the identical messages
        /// -> Send each different message to a list of user tokens
        /// </summary>
        public void Send()
        {
            try
            {
                var lastRunTimestamp = _notificationHistoryRepository.GetLastRunTimestamp();

                _log.Debug($"Getting new notifications not already sent...");
                var notifications = GetAllNewNotificationsNotAlreadySent(lastRunTimestamp);
                
                _log.Debug($"Getting messages for subscribers...");
                var usernotifications = GenerateOneMessageForEachSubscribedUser(notifications);

                _log.Debug($"Getting messages content...");
                var messageContentForEachUser = GetMessageContentForEachUser(usernotifications);

                _log.Debug($"Grouping identical messages...");
                GroupIdenticalMessagesAndSend(messageContentForEachUser);

                RecordRunStatus(notifications, usernotifications);
            }
            catch (Exception ex)
            {
                _log.Error("Error: ", ex);
            }
        }

        private List<PushNotification> GetAllNewNotificationsNotAlreadySent(DateTime lastRunTimestamp)
        {
            return _notificationMakers.SelectMany(maker => maker.GetNotifications(lastRunTimestamp)
              //  .Where(notification => !_notificationHistoryRepository.IsAlreadySent(notification))  // This is now done in the database for speed
              ).ToList();
        }

        private List<IGrouping<string, UserPushNotification>> GenerateOneMessageForEachSubscribedUser(IEnumerable<PushNotification> notifications)
        {
            var usernotifications = notifications.SelectMany(GetUserNotifications).GroupBy(n => n.Token).ToList();
            return usernotifications;
        }

        private void RecordRunStatus(IReadOnlyCollection<PushNotification> notifications, IReadOnlyCollection<IGrouping<string, UserPushNotification>> usernotifications)
        {
            _log.Info($"{notifications.Count} new notifications.");
            _log.Info($"{usernotifications.Select(u => u.Key).Count(u => !string.IsNullOrEmpty(u))} users to send to.");

            _notificationHistoryRepository.AddLog(notifications.Count, usernotifications.Count);
        }
        

        private void GroupIdenticalMessagesAndSend(IEnumerable<UserMessage> userMessages)
        {
            var sameMessages = userMessages.GroupBy(GetGroupKey);
            foreach (var sameMessage in sameMessages)  // 562 ms in GetGroupKey
            {
                var message = sameMessage.First();

                _log.Debug($"Storing in db and getting notificationid...");
                var notificationid = _notificationHistoryRepository.Add(message);

                _log.Debug($"Sending message...");
                var tokens = sameMessage.SelectMany(m => m.Token).Where(m => !string.IsNullOrEmpty(m)).ToList();

                if (tokens.Any())
                {
                    foreach (var batch in tokens.Batch(2000).Select(t => t.ToList()).ToList()) // One signal limited to 2000 playerIds
                    {
                        _pushNotificationSender.Send(notificationid,
                            new UserMessage
                            {
                                PushNotifications = message.PushNotifications,
                                Message = message.Message,
                                Token = batch
                            });
                    }
                }
            }
        }

        private string GetGroupKey(UserMessage userMessage)
        {
            // I believe this may be more efficient than grouping on IEqualityComparer (which has to call the implemented Equals n² times and each call has to enumerate entire list)
            //
            return userMessage.Message
                   + string.Join("i",
                       userMessage.PushNotifications.Where(
                           p => p.PushNotificationType == PushNotificationType.ExpiringInventory)
                                 .Select(k => k.InventoryKey)
                                 .OrderBy(k => k))
                   + string.Join("i",
                       userMessage.PushNotifications.Where(
                           p => p.PushNotificationType == PushNotificationType.ShipmentStatusChanged)
                                 .Select(k => k.ShipmentKey)
                                 .OrderBy(k => k));
        }


        private List<UserMessage> GetMessageContentForEachUser(IEnumerable<IGrouping<string, UserPushNotification>> usernotifications)
        {
            return usernotifications.Select(notification => GetNotificationMessage(notification.Key, notification.ToList())).ToList();
        }

        private IEnumerable<UserPushNotification> GetUserNotifications(PushNotification notification)
        {
            var userTokens = _userSubscriptionRepository.GetUserTokensSubscribedToProtocol(notification.ProtocolId, notification.PushNotificationType);
            if (!userTokens.Any()) userTokens.Add(null); // We stil wabt to record that it's been processed even if there are no users to send to.

            return  userTokens.Select(token => new UserPushNotification(notification, token));
        }

        private UserMessage GetNotificationMessage(string token, IReadOnlyCollection<UserPushNotification> notifications)
        {
            var messages = new List<string>();
            var shipmentNotifications = notifications.Where(n => n.PushNotification.PushNotificationType == PushNotificationType.ShipmentStatusChanged).ToList();
            var inventoryNotifications = notifications.Where(n => n.PushNotification.PushNotificationType == PushNotificationType.ExpiringInventory).ToList();

            if (notifications.Count == 1)
            {
                messages.Add(notifications.Single().PushNotification.Message);
            }
            else
            {
                if (shipmentNotifications.Any()) messages.Add($"{shipmentNotifications.Count} new shipment notification{(shipmentNotifications.Count > 1 ? "s" : "")}");
                if (inventoryNotifications.Any()) messages.Add($"{inventoryNotifications.Count} new inventory notification{(inventoryNotifications.Count > 1 ? "s" : "")}");
            }

            return new UserMessage
            {
                Token = new List<string> { token },
                PushNotifications = notifications.Select(n => n.PushNotification).ToList(),
                Message = string.Join(" and ", messages) + "."
            };
        }
    }
}
