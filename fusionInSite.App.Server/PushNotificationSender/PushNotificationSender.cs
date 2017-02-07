using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using FusionInsite.App.Server.Data.Models;
using log4net;
using Newtonsoft.Json;

namespace FusionInsite.App.Server.PushNotificationSender
{
    public class PushNotificationSender : IPushNotificationSender
    {
        private readonly ILog _log;
        private readonly string _oneSignalAppId = ConfigurationManager.AppSettings["OneSignalAppId"];
        private readonly string _apiKey = ConfigurationManager.AppSettings["OneSignalApiKey"];

        public PushNotificationSender(ILog log)
        {
            _log = log;
        }

        public PushResult Send(int notificationId, UserMessage message)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://onesignal.com/api/v1/notifications");

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("authorization", "Basic " + _apiKey);

            var json = JsonConvert.SerializeObject(new
            {
                app_id = _oneSignalAppId,
                contents = new { en = message.Message},
                data = new { notificationid = notificationId },
                include_player_ids = message.Token.ToArray()
            });


            byte[] byteArray = Encoding.UTF8.GetBytes(json);

            string responseContent;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                var body = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(body);

                dynamic bodyJson = JsonConvert.DeserializeObject(body);
                
                _log.Error("Error sending push notification: " + body, ex);
                return PushResult.Failure;
            }

            
            System.Diagnostics.Debug.WriteLine(responseContent);
            return PushResult.Success;

        }
    }
}
