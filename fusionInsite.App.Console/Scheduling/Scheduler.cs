using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using fusionInsite.App.Console.Scheduling;
using FusionInsite.App.Server;
using log4net;
using Ninject.Syntax;
using Quartz;

namespace fusionInsite.App.Console
{
    public interface IScheduler
    {
        void Start();
        void Stop();
    }


    public class Scheduler : IScheduler
    {
        private readonly ILog _log;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IResolutionRoot _resolutionRoot;
        Quartz.IScheduler _quartzScheduler;


        public Scheduler(ILog log, ISchedulerFactory schedulerFactory, IResolutionRoot resolutionRoot)
        {
            _log = log;
            _schedulerFactory = schedulerFactory;
            _resolutionRoot = resolutionRoot;
        }

        public void Start()
        {
           // var cronExpression = "0/30 * * * * ?";

            //if (!CronExpression.IsValidExpression(cronExpression))
            //{
            //    _log.Warn("Couldn't start the scheduler. Cron expression is invalid.");
            //    return;
            //}

            //if (string.IsNullOrEmpty(cronExpression))
            //{
            //    _log.Warn("No schedule set.");
            //    return;
            //}

            _log.Info("Starting the scheduler...");

            _quartzScheduler = _schedulerFactory.GetScheduler();
            _quartzScheduler.JobFactory = new NinjectJobFactory(_resolutionRoot);
            _quartzScheduler.Start();

            var job = JobBuilder.Create<PushNotificationJob>()
                .WithIdentity("job1", "group1")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                //.WithSchedule(CronScheduleBuilder.CronSchedule(cronExpression))
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(120)
                    .RepeatForever())
         //       .ForJob(job)
                .Build();

            _quartzScheduler.ScheduleJob(job, trigger);
            
        }

        public void Stop() 
        {
            _log.Info("Stopping the scheduler...");
            _schedulerFactory.GetScheduler().Shutdown();
        }
    }
}
