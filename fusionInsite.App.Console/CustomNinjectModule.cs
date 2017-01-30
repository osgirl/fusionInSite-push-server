using FusionInsite.App.Server.GetNewNotifications;
using Ninject.Extensions.Conventions;
using Ninject.Modules;

namespace fusionInsite.App.Console
{
    internal class CustomNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(x =>
            {
                x.FromAssembliesMatching("*")
                    .SelectAllClasses()      // Retrieve all non-abstract classes
                    .BindDefaultInterface(); // Binds the default interface to them;
            });

            Bind<IGetNewNotifications>().To<GetNewNotificationsShipmentChanged>();
            Bind<IGetNewNotifications>().To<GetNewNotificationsNotReceivedShipment>();
            Bind<IGetNewNotifications>().To<GetNewNotificationsExpiringInventory>();
            
        }
    }
}