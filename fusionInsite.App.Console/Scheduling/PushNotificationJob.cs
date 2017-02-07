using FusionInsite.App.Server;
using log4net;
using Quartz;

namespace fusionInsite.App.Console.Scheduling
{
    [DisallowConcurrentExecution]
    public class PushNotificationJob : IJob
    {
        private readonly ILog _log;
        private readonly IPushNotificationCoordinator _pushNotificationCoordinator;

        public PushNotificationJob(ILog log, IPushNotificationCoordinator pushNotificationCoordinator)
        {
            _log = log;
            _pushNotificationCoordinator = pushNotificationCoordinator;
        }

        public void Execute(IJobExecutionContext context)
        {
            _log.Info("Executing job...");

            _pushNotificationCoordinator.Send();

            _log.Info("Completed job.");
        }
    }
}
