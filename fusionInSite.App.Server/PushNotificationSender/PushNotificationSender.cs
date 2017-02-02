using System.IO;
using System.Net;
using System.Text;
using FusionInsite.App.Server.Data.Models;
using Newtonsoft.Json;

namespace FusionInsite.App.Server.PushNotificationSender
{
    public class PushNotificationSender : IPushNotificationSender
    {
        public PushResult Send(UserMessage message)
        {
            var request = (HttpWebRequest)WebRequest.Create("https://onesignal.com/api/v1/notifications");

            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";

            request.Headers.Add("authorization", "Basic NjI5N2Y2NTktNWNjOC00YTM1LTk5MDItMWJlNmRhYzM1YzE2");

            var json = JsonConvert.SerializeObject(new
            {
                app_id = "c0231a37-8fd2-4d4b-8185-5d0f4b6491cd",
                contents = new { en = message},
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



                return PushResult.Failure;
            }

            
            System.Diagnostics.Debug.WriteLine(responseContent);
            return PushResult.Success;

        }
    }
}
