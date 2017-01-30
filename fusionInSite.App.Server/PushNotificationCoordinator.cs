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
        private readonly ISentNotificationRepository _sentNotificationRepository;
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly IEnumerable<IGetNewNotifications> _notificationMakers;
        private readonly IPushNotificationSender _pushNotificationSender;

        public PushNotificationCoordinator(ISentNotificationRepository sentNotificationRepository, IUserSubscriptionRepository userSubscriptionRepository, IEnumerable<IGetNewNotifications> notificationMakers, IPushNotificationSender pushNotificationSender) // Ninject will bind all instances
        {
            _sentNotificationRepository = sentNotificationRepository;
            _userSubscriptionRepository = userSubscriptionRepository;
            _notificationMakers = notificationMakers;
            _pushNotificationSender = pushNotificationSender;
        }

        public void Send()
        {
            var notifications = _notificationMakers.SelectMany(maker => maker.GetNotifications()
                .Where(notification => !_sentNotificationRepository.IsAlreadySent(notification)))
                .ToList();
            
            var usernotifications = notifications.SelectMany(GetUserNotifications).GroupBy(n => n.User);

            foreach (var notification in usernotifications)
            {
                SendNotification(notification.Key, notification.ToList());
            }

            foreach (var notification in notifications) _sentNotificationRepository.Add(notification);
        }

        private IEnumerable<UserPushNotification> GetUserNotifications(PushNotification notification)
        {
            return _userSubscriptionRepository.GetUsersSubscribedToProtocol(notification.ProtocolId)
                .Select(user => (new UserPushNotification(notification)).WithUser(user));
        }

        private void SendNotification(string user, IReadOnlyCollection<UserPushNotification> notifications)
        {
            var userMessage = GetNotification(user, notifications);
            PushResult result = _pushNotificationSender.Send(userMessage);
        }

        private UserMessage GetNotification(string user, IReadOnlyCollection<UserPushNotification> notifications)
        {
            if (notifications.Count == 1)
            {
                return new UserMessage {User = user, Message = notifications.Single().Message};
            }

            return new UserMessage
            {
                User = user,
                Message = "Multiple messages"
            };
        }
    }
}
