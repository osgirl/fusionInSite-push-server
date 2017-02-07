using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

                var notifications = GetAllNewNotificationsNotAlreadySent(lastRunTimestamp);
                var usernotifications = GenerateOneMessageForEachSubscribedUser(notifications);

                var messageContentForEachUser = GetMessageContentForEachUser(usernotifications);
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
                .Where(notification => !_notificationHistoryRepository.IsAlreadySent(notification)))
                .ToList();
        }

        private List<IGrouping<string, UserPushNotification>> GenerateOneMessageForEachSubscribedUser(IEnumerable<PushNotification> notifications)
        {
            var usernotifications = notifications.SelectMany(GetUserNotifications).GroupBy(n => n.Token).ToList();
            return usernotifications;
        }

        private void RecordRunStatus(IReadOnlyCollection<PushNotification> notifications, IReadOnlyCollection<IGrouping<string, UserPushNotification>> usernotifications)
        {
            _log.Info($"{notifications.Count} new notifications.");
            _log.Info($"{usernotifications.Count} users to send to.");

            _notificationHistoryRepository.AddLog(notifications.Count, usernotifications.Count);
        }
        

        private void GroupIdenticalMessagesAndSend(IEnumerable<UserMessage> userMessages)
        {
            foreach (var sameMessage in userMessages.GroupBy(GetGroupKey))
            {
                var message = sameMessage.First();

                var notificationid = _notificationHistoryRepository.Add(message);
                _pushNotificationSender.Send(notificationid,
                    new UserMessage
                    {
                        PushNotifications = message.PushNotifications,
                        Message = message.Message,
                        InventoryKeys = message.InventoryKeys,
                        ShipmentKeys = message.ShipmentKeys,
                        Token = sameMessage.SelectMany(m => m.Token).ToList()
                    });
            }
        }

        private string GetGroupKey(UserMessage userMessage)
        {
            // I believe this may be more efficient than grouping on IEqualityComparer (which has to call the implemented Equals n² times and each call has to enumerate entire list)
            //
            return userMessage.Message + string.Join("i", userMessage.InventoryKeys.OrderBy(k => k)) + string.Join("s", userMessage.ShipmentKeys.OrderBy(k => k));
        }


        private List<UserMessage> GetMessageContentForEachUser(IEnumerable<IGrouping<string, UserPushNotification>> usernotifications)
        {
            return usernotifications.Select(notification => GetNotificationMessage(notification.Key, notification.ToList())).ToList();
        }

        private IEnumerable<UserPushNotification> GetUserNotifications(PushNotification notification)
        {
            return _userSubscriptionRepository.GetUserTokensSubscribedToProtocol(notification.ProtocolId, notification.PushNotificationType)
                .Select(token => (new UserPushNotification(notification)).WithUserToken(token));
        }
        

        private UserMessage GetNotificationMessage(string token, IReadOnlyCollection<UserPushNotification> notifications)
        {
            var messages = new List<string>();
            var shipmentNotifications = notifications.Where(n => n.PushNotificationType == PushNotificationType.ShipmentStatusChanged).ToList();
            var inventoryNotifications = notifications.Where(n => n.PushNotificationType == PushNotificationType.ExpiringInventory).ToList();

            if (notifications.Count == 1)
            {
                messages.Add(notifications.Single().Message);
            }
            else
            {
                if (shipmentNotifications.Any()) messages.Add($"{shipmentNotifications.Count} new shipment notification{(shipmentNotifications.Count > 1 ? "s" : "")}");
                if (inventoryNotifications.Any()) messages.Add($"{inventoryNotifications.Count} new inventory notification{(inventoryNotifications.Count > 1 ? "s" : "")}");
            }

            return new UserMessage
            {
                Token = new List<string> { token},
                ShipmentKeys = shipmentNotifications.Select(n => n.ShipmentKey).ToList(),
                InventoryKeys = inventoryNotifications.Select(n => n.InventoryKey).ToList(),
                PushNotifications = notifications.Cast<PushNotification>().ToList(),
                Message = string.Join(" and ", messages) + "."
            };
        }
    }
}
