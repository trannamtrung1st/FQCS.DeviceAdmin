using Confluent.Kafka;
using FQCS.DeviceAdmin.Business.Models;
using FQCS.DeviceAdmin.Data.Models;
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
        public DeviceConfig CurrentConfig { get; set; }
        public IProducer<Null, string> KafkaProducer { get; set; }
        public string QCEventImageFolderPath { get; set; }
        public string StatePath { get; set; }

        public RemoveOldEventsJobSettings RemoveOldEventsJobSettings { get; }
        public SendUnsentEventsJobSettings SendUnsentEventsJobSettings { get; }
        protected readonly IServiceProvider serviceProvider;


        public FQCSScheduler(IServiceProvider serviceProvider)
        {
            RemoveOldEventsJobSettings = new RemoveOldEventsJobSettings();
            SendUnsentEventsJobSettings = new SendUnsentEventsJobSettings();
            this.serviceProvider = serviceProvider;
        }

        // static
        public static readonly JobKey RemoveOldEventsJobKey
            = new JobKey(nameof(RemoveOldEventsJob), Constants.Group.General);
        public static readonly TriggerKey RemoveOldEventsTriggerKey
            = new TriggerKey(nameof(RemoveOldEventsJob), Constants.Group.General);
        public static readonly JobKey SendUnsentEventsJobKey
            = new JobKey(nameof(SendUnsentEventsJob), Constants.Group.General);
        public static readonly TriggerKey SendUnsentEventsJobTriggerKey
            = new TriggerKey(nameof(SendUnsentEventsJob), Constants.Group.General);

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
                    { Constants.CommonDataKey.FQCS_SCHEDULER, this },
                    { Constants.CommonDataKey.SETTINGS, RemoveOldEventsJobSettings },
                    { Constants.CommonDataKey.SERVICE_PROVIDER, serviceProvider },
                    { Constants.CommonDataKey.JOB_INFO, new JobInfo() }
                }).StoreDurably(true).Build();
            await Scheduler.AddJob(job, true);

            job = JobBuilder.Create<SendUnsentEventsJob>()
                .WithIdentity(SendUnsentEventsJobKey)
                .UsingJobData(new JobDataMap()
                {
                    { Constants.CommonDataKey.FQCS_SCHEDULER, this },
                    { Constants.CommonDataKey.SETTINGS, SendUnsentEventsJobSettings },
                    { Constants.CommonDataKey.SERVICE_PROVIDER, serviceProvider },
                    { Constants.CommonDataKey.JOB_INFO, new JobInfo() }
                }).StoreDurably(true).Build();
            await Scheduler.AddJob(job, true);
        }

        public async Task<DateTimeOffset?> TriggerSendUnsentEventsJob(DateTime? startAt)
        {
            ValidateSendUnsentJobSchedule();
            TriggerBuilder builder = TriggerBuilder.Create()
                .WithIdentity(nameof(SendUnsentEventsJob) + Guid.NewGuid().ToString(), Constants.Group.General);
            if (startAt != null)
                builder = builder.StartAt(startAt.Value);
            else builder = builder.StartNow();
            var trigger = builder.ForJob(SendUnsentEventsJobKey).Build();
            var oldTrigger = await Scheduler.GetTrigger(SendUnsentEventsJobTriggerKey);
            if (oldTrigger != null)
                return await Scheduler.RescheduleJob(SendUnsentEventsJobTriggerKey, trigger);
            return await Scheduler.ScheduleJob(trigger);
        }

        public async Task<DateTimeOffset?> ScheduleSendUnsentEventsJob(int intervalSecs, DateTime? startAt)
        {
            ValidateSendUnsentJobSchedule();
            TriggerBuilder builder = TriggerBuilder.Create()
                .WithIdentity(SendUnsentEventsJobTriggerKey);
            if (startAt != null)
                builder = builder.StartAt(startAt.Value);
            else builder = builder.StartNow();
            builder = builder.WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromSeconds(intervalSecs))
                    .RepeatForever());
            var trigger = builder.ForJob(SendUnsentEventsJobKey).Build();
            var oldTrigger = await Scheduler.GetTrigger(SendUnsentEventsJobTriggerKey);
            if (oldTrigger != null)
                return await Scheduler.RescheduleJob(SendUnsentEventsJobTriggerKey, trigger);
            return await Scheduler.ScheduleJob(trigger);
        }

        public async Task<bool> UnscheduleSendUnsentEventsJob()
        {
            return await Scheduler.UnscheduleJob(SendUnsentEventsJobTriggerKey);
        }

        public async Task<DateTimeOffset?> ScheduleRemoveOldEventsJob(int intervalSecs, DateTime? startAt)
        {
            ValidateRemoveOldEventsJobSchedule();
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

        #region Validation
        public void ValidateRemoveOldEventsJobSchedule()
        {
            if (RemoveOldEventsJobSettings.KeepDays == null)
                throw new Exception("Invalid settings");
        }

        public void ValidateSendUnsentJobSchedule()
        {
            if (CurrentConfig == null || KafkaProducer == null)
                throw new Exception("Invalid settings");
        }
        #endregion

    }
}
