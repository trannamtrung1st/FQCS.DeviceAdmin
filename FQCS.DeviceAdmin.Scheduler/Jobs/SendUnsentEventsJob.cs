﻿using Confluent.Kafka;
using FQCS.DeviceAdmin.Business;
using FQCS.DeviceAdmin.Business.Models;
using FQCS.DeviceAdmin.Business.Queries;
using FQCS.DeviceAdmin.Business.Services;
using FQCS.DeviceAdmin.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.Scheduler.Jobs
{
    public class SendUnsentEventsJob : BaseJob
    {
        public override async Task Execute(IJobExecutionContext context)
        {
            await base.Execute(context);
            var data = context.JobDetail.JobDataMap;
            var scheduler = data[Constants.CommonDataKey.FQCS_SCHEDULER] as FQCSScheduler;
            var settings = data[Constants.CommonDataKey.SETTINGS] as SendUnsentEventsJobSettings;
            var provider = data[Constants.CommonDataKey.SERVICE_PROVIDER] as IServiceProvider;
            using var scope = provider.CreateScope();
            provider = scope.ServiceProvider;
            var dContext = provider.GetRequiredService<DataContext>();
            var qcEventService = provider.GetRequiredService<IQCEventService>();
            var query = qcEventService.QCEvents.Include(o => o.Details).AsQueryable();
            if (State.Instance.LastEventTime != null)
                query = query.FromTime(State.Instance.LastEventTime, true);
            var entities = query.ToList();
            foreach (var entity in entities)
            {
                if (scheduler.KafkaProducer != null)
                {
                    qcEventService.ProduceEventToKafkaServer(scheduler.KafkaProducer,
                        entity, scheduler.CurrentConfig, scheduler.QCEventImageFolderPath,
                        scheduler.StatePath);
                    if (settings.SleepSecs != null)
                        Thread.Sleep(settings.SleepSecs.Value * 1000);
                }
            }
        }
    }

}
