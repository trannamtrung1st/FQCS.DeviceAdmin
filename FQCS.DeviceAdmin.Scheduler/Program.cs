using FQCS.DeviceAdmin.Data.Models;
using FQCS.DeviceAdmin.Scheduler.Jobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TNT.Core.Helpers.DI;

namespace FQCS.DeviceAdmin.Scheduler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var services = new ServiceCollection();
            //services.AddSingleton(new TenantInfo { TenantId = Guid.NewGuid().ToString() });
            Data.Global.Init(services);
            Business.Global.Init(services);
            services.AddServiceInjection();
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(Data.Constants.Data.CONN_STR)
                    .UseLazyLoadingProxies());
            var provider = services.BuildServiceProvider();
            using var fqcs = new FQCSScheduler(provider);
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
