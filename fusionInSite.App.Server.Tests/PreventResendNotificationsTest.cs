using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
using FusionInsite.App.Server.Data.Models;
using FusionInsite.App.Server.Data.Repositories;
using FusionInsite.App.Server.Data.Repositories.Interfaces;
using FusionInsite.App.Server.GetNewNotifications;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FusionInsite.App.Server.Tests
{
    [TestClass]
    public class PreventResendNotificationsTest
    {
        private readonly PushNotification _notificationProtocol1Id1 = new PushNotification
        {
            PushNotificationType = PushNotificationType.NotReceivedShipment,
            ProtocolId = 101,
            Id = 1
        };

        private readonly PushNotification _notificationProtocol1Id2 = new PushNotification
        {
            PushNotificationType = PushNotificationType.NotReceivedShipment,
            ProtocolId = 101,
            Id = 2
        };
        
        private readonly PushNotification _notificationProtocol2 = new PushNotification
        {
            PushNotificationType = PushNotificationType.NotReceivedShipment,
            ProtocolId = 102,
            Id = 1
        };
        
        [TestMethod]
        public void WithNoNotificationsToSend_Send_DoesNotSendTheNotification()
        {
            var builder = new PushNotificationCoordinatorBuilder();
            var push = builder.Build();
            push.Send();
            builder.AssetSendsNotifications(0);
        }

        [TestMethod]
        public void WithNewNotificationsToSend_Send_SendsTheNotification()
        {
            var builder =
                new PushNotificationCoordinatorBuilder().WithNotificationsToSend(_notificationProtocol1Id1);
            var push = builder.Build();
            push.Send();
            builder.AssetSendsNotifications(1);
        }
        [TestMethod]
        public void WithNewNotificationsToSend_Send_AddsToSentRepository()
        {
            var builder =
                new PushNotificationCoordinatorBuilder().WithNotificationsToSend(_notificationProtocol1Id1);
            var push = builder.Build();
            push.Send();
            builder.AssertAddsToRepo(_notificationProtocol1Id1);
        }

        [TestMethod]
        public void WithNotificationThatWasAlreadySent_Send_DoesNotSendTheNotification()
        {
            var builder =
                new PushNotificationCoordinatorBuilder()
                    .WithNotificationsToSend(_notificationProtocol1Id2)
                    .WithNotificationsAlreadySent(_notificationProtocol1Id2);
            var push = builder.Build();
            push.Send();
            builder.AssetSendsNotifications(0);
        }
        
        [TestMethod]
        public void WithOneNotificationTo2DifferentUsers_Send_SendsBoth()
        {
            var builder =
                new PushNotificationCoordinatorBuilder().WithNotificationsToSend(_notificationProtocol1Id1)
                .WithUserSubscribedToProtocol(101, new List<string> {"user1", "user2"});
            var push = builder.Build();
            push.Send();
            builder.AssetSendsNotifications(2);
        }
        
        [TestMethod]
        public void WithTwoNotificationTo2DifferentUsers_Send_SendsBoth()
        {
            var builder =
                new PushNotificationCoordinatorBuilder()
                    .WithNotificationsToSend(_notificationProtocol1Id1)
                    .WithNotificationsToSend(_notificationProtocol1Id2)
                    .WithUserSubscribedToProtocol(101, new List<string> { "user1", "user2" });
            var push = builder.Build();
            push.Send();
            builder.AssetSendsNotifications(2);
        }
        
        [TestMethod]
        public void WithTwoNotificationsToDSameUser_Send_GroupsTheNotifications()
        {
            var builder =
                new PushNotificationCoordinatorBuilder().WithNotificationsToSend(_notificationProtocol1Id1).WithNotificationsToSend(_notificationProtocol1Id2);
            var push = builder.Build();
            push.Send();
            builder.AssetSendsNotifications(1);
        }

        
        [TestMethod]
        public void WithTwoNotificationsToTheSameUserOneAlreadySent_Send_SendsOne()
        {
            var builder =
                new PushNotificationCoordinatorBuilder()
                .WithNotificationsToSend(_notificationProtocol1Id1)
                .WithNotificationsToSend(_notificationProtocol1Id2)
                .WithNotificationsAlreadySent(_notificationProtocol1Id1);
            var push = builder.Build();
            push.Send();
            builder.AssetSendsNotifications(1);
        }
        
        [TestMethod]
        public void WithTwoNotificationsToTheSameUserBothAlreadySent_Send_DoesNotSendAny()
        {
            var builder =
                new PushNotificationCoordinatorBuilder()
                .WithNotificationsToSend(_notificationProtocol1Id1)
                .WithNotificationsToSend(_notificationProtocol1Id2)
                .WithNotificationsAlreadySent(_notificationProtocol1Id1)
                .WithNotificationsAlreadySent(_notificationProtocol1Id2);
            var push = builder.Build();
            push.Send();
            builder.AssetSendsNotifications(0);
        }

        [TestMethod]
        public void WithTwoNotificationsSubscribedByDifferentUsersOneAlreadySent_Send_SendsOne()
        {
            var builder =
                new PushNotificationCoordinatorBuilder()
                    .WithNotificationsToSend(_notificationProtocol1Id1)
                    .WithNotificationsToSend(_notificationProtocol2)
                    .WithNotificationsAlreadySent(_notificationProtocol1Id1)
                    .WithUserSubscribedToProtocol(101, new List<string> {"user1"})
                    .WithUserSubscribedToProtocol(102, new List<string> {"user2"});
            var push = builder.Build();
            push.Send();
            builder.AssetSendsNotifications(1);
        }


    }

    public class PushNotificationCoordinatorBuilder
    {
        private readonly Mock<INotificationHistoryRepository> _sentNotificationRepository = new Mock<INotificationHistoryRepository>();
        private readonly Mock<IPushNotificationSender> _pushNotificationSender = new Mock<IPushNotificationSender>();
        private readonly Mock<IGetNewNotifications> _getNewNotifications = new Mock<IGetNewNotifications>();

        private readonly List<PushNotification> _pushNotifications = new List<PushNotification>();
        private readonly Mock<IUserSubscriptionRepository> _userNotificationRepository = new Mock<IUserSubscriptionRepository>();

        public PushNotificationCoordinatorBuilder()
        {
            _userNotificationRepository.Setup(r => r.GetUsersSubscribedToProtocol(It.IsAny<int>()))
                .Returns(new List<string> {"user1"});
            _pushNotificationSender.Setup(s => s.Send(It.IsAny<UserMessage>())).Returns(PushResult.Success);
        }



        public PushNotificationCoordinator Build()
        {
            _getNewNotifications.Setup(g => g.GetNotifications()).Returns(_pushNotifications);
            
            return new PushNotificationCoordinator(_sentNotificationRepository.Object, _userNotificationRepository.Object, new List<IGetNewNotifications>
            {
                _getNewNotifications.Object
            }, _pushNotificationSender.Object);
        }

        public void AssetSendsNotifications(int number)
        {
            _pushNotificationSender.Verify(s => s.Send(It.IsAny<UserMessage>()), Times.Exactly(number));
        }
        public void AssertAddsToRepo(PushNotification notification)
        {
            _sentNotificationRepository.Verify(s => s.Add(notification));
        }

        public PushNotificationCoordinatorBuilder WithNotificationsToSend(PushNotification pushNotification)
        {
            _pushNotifications.Add(pushNotification);
            return this;
        }

        public PushNotificationCoordinatorBuilder WithUserSubscribedToProtocol(int protocolId, List<string> users)
        {
            _userNotificationRepository.Setup(r => r.GetUsersSubscribedToProtocol(protocolId)).Returns(users);
            return this;
        }

        public PushNotificationCoordinatorBuilder WithNotificationsAlreadySent(PushNotification notification)
        {
            _sentNotificationRepository.Setup(
                r =>
                    r.IsAlreadySent(
                        It.Is<PushNotification>(n => n.PushNotificationType == notification.PushNotificationType
                                                     && n.ProtocolId == notification.ProtocolId
                                                     && n.Id == notification.Id)))
                .Returns(true);
            return this;
        }
    }

}
