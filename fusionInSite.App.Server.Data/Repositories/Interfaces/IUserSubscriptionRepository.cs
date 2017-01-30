using System.Collections.Generic;

namespace FusionInsite.App.Server.Data.Repositories.Interfaces
{
    public interface IUserSubscriptionRepository
    {
        List<string> GetUsersSubscribedToProtocol(string protocolId);
        void SubscribeUser(string protocolId, string user);
        void UnsubscribeUser(string protocolId, string user);
    }

    public class UserSubscriptionRepository : IUserSubscriptionRepository
    {
        public List<string> GetUsersSubscribedToProtocol(string protocolId)
        {
            throw new System.NotImplementedException();
        }

        public void SubscribeUser(string protocolId, string user)
        {
            throw new System.NotImplementedException();
        }

        public void UnsubscribeUser(string protocolId, string user)
        {
            throw new System.NotImplementedException();
        }
    }
}
