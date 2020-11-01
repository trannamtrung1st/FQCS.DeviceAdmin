using FQCS.DeviceAdmin.Scheduler.Jobs;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FQCS.DeviceAdmin.Scheduler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var fqcs = new FQCSScheduler();
            fqcs.Start().Wait();
            var jKey = new JobKey(
                nameof(RemoveOldEventsJob), Constants.Group.General);
            var tKey = new TriggerKey(jKey.Name, jKey.Group);
            var jobDetail = fqcs.Scheduler.GetJobDetail(jKey).Result;
            var jobInfo = jobDetail.JobDataMap[Constants.CommonDataKey.JOB_INFO] as JobInfo;
            fqcs.RemoveOldEventsJobSettings.KeepDays = 70;
            var dt = fqcs.ScheduleRemoveOldEventsJob(5, DateTime.UtcNow.AddSeconds(10)).Result;
            Console.WriteLine(dt.Value.ToLocalTime());
            while (jobInfo.ExecutionCount < 3)
            {
                Console.WriteLine(jobInfo.ExecutionCount);
                Thread.Sleep(5000);
            }
            fqcs.Scheduler.UnscheduleJob(tKey);
            fqcs.Scheduler.Shutdown();
        }
    }
}
