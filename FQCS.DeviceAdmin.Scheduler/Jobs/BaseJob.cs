using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FQCS.DeviceAdmin.Scheduler.Jobs
{
    public abstract class BaseJob : IJob
    {
        protected void PrepareExecute(IJobExecutionContext context)
        {
            var data = context.JobDetail.JobDataMap;
            var info = data[Constants.CommonDataKey.JOB_INFO] as JobInfo;
            info.ExecutionCount++;
        }

        public virtual Task Execute(IJobExecutionContext context)
        {
            PrepareExecute(context);
            return Task.CompletedTask;
        }

    }
}
