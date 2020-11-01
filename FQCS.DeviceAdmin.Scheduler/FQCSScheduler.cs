using FQCS.DeviceAdmin.Scheduler.Jobs;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.Scheduler
{
    public class FQCSScheduler : IDisposable
    {
        public IScheduler Scheduler { get; private set; }
        public RemoveOldEventsJobSettings RemoveOldEventsJobSettings { get; }

        public FQCSScheduler()
        {
            RemoveOldEventsJobSettings = new RemoveOldEventsJobSettings();
        }

        // static
        public static readonly JobKey RemoveOldEventsJobKey
            = new JobKey(nameof(RemoveOldEventsJob), Constants.Group.General);
        public static readonly TriggerKey RemoveOldEventsTriggerKey
            = new TriggerKey(nameof(RemoveOldEventsJob), Constants.Group.General);

        public async Task Start()
        {
            // construct a scheduler factory
            StdSchedulerFactory factory = new StdSchedulerFactory();
            // get a scheduler
            Scheduler = await factory.GetScheduler();
            await Scheduler.Start();

            IJobDetail job = JobBuilder.Create<RemoveOldEventsJob>()
                .WithIdentity(RemoveOldEventsJobKey)
                .UsingJobData(new JobDataMap()
                {
                    { Constants.CommonDataKey.SETTINGS, RemoveOldEventsJobSettings },
                    { Constants.CommonDataKey.JOB_INFO, new JobInfo() }
                }).StoreDurably(true).Build();
            await Scheduler.AddJob(job, true);
        }

        public async Task<DateTimeOffset?> ScheduleRemoveOldEventsJob(int intervalSecs, DateTime? startAt)
        {
            TriggerBuilder builder = TriggerBuilder.Create()
                .WithIdentity(RemoveOldEventsTriggerKey);
            if (startAt != null)
                builder = builder.StartAt(startAt.Value);
            else builder = builder.StartNow();
            builder = builder.WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromSeconds(intervalSecs))
                    .RepeatForever());
            var trigger = builder.ForJob(RemoveOldEventsJobKey).Build();
            var oldTrigger = await Scheduler.GetTrigger(RemoveOldEventsTriggerKey);
            if (oldTrigger != null)
                return await Scheduler.RescheduleJob(RemoveOldEventsTriggerKey, trigger);
            return await Scheduler.ScheduleJob(trigger);
        }

        public async Task<bool> UnscheduleRemoveOldEventsJob()
        {
            return await Scheduler.UnscheduleJob(RemoveOldEventsTriggerKey);
        }

        public void Dispose()
        {
            Scheduler.Shutdown().Wait();
        }
    }
}
