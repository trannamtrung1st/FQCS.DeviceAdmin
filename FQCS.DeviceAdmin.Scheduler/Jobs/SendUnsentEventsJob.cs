using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
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
            Console.WriteLine("Triggerd");
        }
    }

    public class SendUnsentEventsJobSettings
    {
    }
}
