namespace FusionInsite.App.Server.Data.Models
{
    public class ProtocolSubscription
    {
        public string ProtocolId { get; set; }
        public string UserId { get; set; }

        public ProtocolSubscription(string protocolId, string userId)
        {
            ProtocolId = protocolId;
            UserId = userId;
        }
    }
}
