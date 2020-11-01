using Confluent.Kafka;
using FQCS.DeviceAdmin.Business.Queries;
using FQCS.DeviceAdmin.Business.Services;
using FQCS.DeviceAdmin.Data.Models;
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
            var settings = data[Constants.CommonDataKey.SETTINGS] as SendUnsentEventsJobSettings;
            var provider = data[Constants.CommonDataKey.SERVICE_PROVIDER] as IServiceProvider;
            using var scope = provider.CreateScope();
            provider = scope.ServiceProvider;
            var dContext = provider.GetRequiredService<DataContext>();
            var qcEventService = provider.GetRequiredService<QCEventService>();
            var entities = qcEventService.QCEvents.Unsent().ToList();
            foreach (var entity in entities)
            {
                if (settings.SleepSecs != null)
                    Thread.Sleep(settings.SleepSecs.Value * 1000);
                if (settings.KafkaProducer != null)
                    qcEventService.ProduceEventToKafkaServer(settings.KafkaProducer,
                        entity, settings.CurrentConfig, settings.QCEventImageFolderPath,
                        settings.ConnStr);
            }
        }
    }

    public class SendUnsentEventsJobSettings
    {
        public int? SleepSecs { get; set; }
        public DeviceConfig CurrentConfig { get; set; }
        public IProducer<Null, string> KafkaProducer { get; set; }
        public string QCEventImageFolderPath { get; set; }
        public string ConnStr { get; set; }
    }
}
