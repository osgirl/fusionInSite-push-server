using System.Threading;
using FusionInsite.App.Server;
using FusionInsite.App.Server.Data.Repositories;
using log4net;
using Ninject;
using Quartz;
using Topshelf;

namespace fusionInsite.App.Console
{
    internal class Program
    {
        private readonly ILog _log;
        private readonly IScheduler _scheduler;

        public Program(ILog log, IScheduler scheduler)
        {
            _log = log;
            _scheduler = scheduler;
        }

        private static void Main(string[] args)
        {
            IKernel kernel = new StandardKernel(new CustomNinjectModule());

            Log4NetConfiguration.Configure();

            HostFactory.Run(x =>
            {
                //x.UseLog4Net();
                x.Service<Program>(c =>
                {
                    c.ConstructUsing(name => kernel.Get<Program>());
                    c.WhenStarted(p => new Thread(p.Start).Start()); // use a new thread to avoid timeouts in starting the service
                    c.WhenStopped(p => p.Stop());
                });

                x.RunAsLocalSystem();
                x.SetServiceName("fusionInSite.App.Server");
                x.SetDisplayName("fusionInSite App Server");
            });

            System.Console.ReadLine();
        }

   
        void Start()
        {
            _log.Info("fusionInSite.App.Server is starting...");
            _scheduler.Start();
        }



        void Stop()
        {
            _log.Info("fusionInSite.App.Server is stopping...");
            _scheduler.Stop();
        }
    }
}