using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FusionInsite.App.Server.Data.Models;
using FusionInsite.App.Server.Data.Repositories;
using FusionInsite.App.Server.Data.Repositories.Interfaces;
using FusionInsite.App.Server.GetNewNotifications;

namespace FusionInsite.App.Server
{
    public class PushNotificationCoordinator
    {
        private readonly INotificationHistoryRepository _notificationHistoryRepository;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly IEnumerable<IGetNewNotifications> _notificationMakers;
        private readonly IPushNotificationSender _pushNotificationSender;

        public PushNotificationCoordinator(INotificationHistoryRepository notificationHistoryRepository, IUserSubscriptionRepository userSubscriptionRepository, IEnumerable<IGetNewNotifications> notificationMakers, IPushNotificationSender pushNotificationSender) // Ninject will bind all instances
        {
            _notificationHistoryRepository = notificationHistoryRepository;
            _userSubscriptionRepository = userSubscriptionRepository;
            _notificationMakers = notificationMakers;
            _pushNotificationSender = pushNotificationSender;
        }

        public void Send()
        {
            var lastRunTimestamp = _notificationHistoryRepository.GetLastRunTimestamp();

            var notifications = _notificationMakers.SelectMany(maker => maker.GetNotifications(lastRunTimestamp)
                .Where(notification => !_notificationHistoryRepository.IsAlreadySent(notification)))
                .ToList();

            Console.WriteLine($"{notifications.Count} new notifications.");

            var usernotifications = notifications.SelectMany(GetUserNotifications).GroupBy(n => n.Token).ToList();

            Console.WriteLine($"{usernotifications.Count} users to send to.");

            foreach (var notification in usernotifications)
            {
                SendNotification(notification.Key, notification.ToList());
            }

            foreach (var notification in notifications) _notificationHistoryRepository.Add(notification);

            _notificationHistoryRepository.AddLog(notifications.Count, usernotifications.Count);
        }

        private IEnumerable<UserPushNotification> GetUserNotifications(PushNotification notification)
        {
            return _userSubscriptionRepository.GetUserTokensSubscribedToProtocol(notification.ProtocolId, notification.PushNotificationType)
                .Select(token => (new UserPushNotification(notification)).WithUserToken(token));
        }

        private void SendNotification(string token, IReadOnlyCollection<UserPushNotification> notifications)
        {
            var userMessage = GetNotificationMessage(token, notifications);
            PushResult result = _pushNotificationSender.Send(userMessage);
        }

        private UserMessage GetNotificationMessage(string token, IReadOnlyCollection<UserPushNotification> notifications)
        {
            if (notifications.Count == 1)
            {
                return new UserMessage {Token = token, Message = notifications.Single().Message};
            }

            return new UserMessage
            {
                Token = token,
                Message = "Multiple messages"
            };
        }
    }
}
