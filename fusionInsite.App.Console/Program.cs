using FusionInsite.App.Server;
using FusionInsite.App.Server.Data.Repositories;
using Ninject;

namespace fusionInsite.App.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IKernel kernel = new StandardKernel(new CustomNinjectModule());
            
            var pushNotificationCoordinator = kernel.Get<PushNotificationCoordinator>();

            pushNotificationCoordinator.Send();

            System.Console.ReadLine();
        }
    }
}