namespace FusionInsite.App.Server.Data.Models
{
    public class UserPushNotification
    {
        public UserPushNotification(PushNotification pushNotification, string token)
        {
            PushNotification = pushNotification;
            Token = token;
        }

        public PushNotification PushNotification { get; set; }

        public string Token { get; set; }

        public UserPushNotification WithUserToken(string token)
        {
            Token = token;
            return this;
        }
    }
}