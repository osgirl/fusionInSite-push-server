using FusionInsite.App.Server.GetNewNotifications;
using log4net;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using Quartz;
using Quartz.Impl;

namespace fusionInsite.App.Console
{
    internal class CustomNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind(x =>
            {
                x.FromAssembliesMatching("fusionInsite.*")
                 .SelectAllClasses()      // Retrieve all non-abstract classes
                 .BindDefaultInterface(); // Binds the default interface to them;
            });

            Bind<IGetNewNotifications>().To<GetNewNotificationsShipmentChanged>();
            Bind<IGetNewNotifications>().To<GetNewNotificationsExpiringInventory>();

            Bind<ISchedulerFactory>().To<StdSchedulerFactory>();

            // Bind<IGetNewNotifications>().To<GetNewNotificationsNotReceivedShipment>();

            Bind<ILog>().ToMethod(x => LogManager.GetLogger("fusionInSite.App.Server"));
        }
    }
}