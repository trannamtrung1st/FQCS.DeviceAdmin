using FQCS.DeviceAdmin.Business.Queries;
using FQCS.DeviceAdmin.Business.Services;
using FQCS.DeviceAdmin.Data.Models;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.Scheduler.Jobs
{
    public class RemoveOldEventsJob : BaseJob
    {

        public override async Task Execute(IJobExecutionContext context)
        {
            await base.Execute(context);
            var data = context.JobDetail.JobDataMap;
            var settings = data[Constants.CommonDataKey.SETTINGS] as RemoveOldEventsJobSettings;
            var provider = data[Constants.CommonDataKey.SERVICE_PROVIDER] as IServiceProvider;
            using var scope = provider.CreateScope();
            provider = scope.ServiceProvider;
            var dContext = provider.GetRequiredService<DataContext>();
            var qcEventService = provider.GetRequiredService<QCEventService>();
            var entities = qcEventService.QCEvents.ExceptLast(settings.KeepDays);
            var removed = qcEventService.DeleteQCEvents(entities);
            Console.WriteLine($"Removed: {removed}");
        }
    }

    public class RemoveOldEventsJobSettings
    {
        public int KeepDays { get; set; }
    }
}
