using System;
using System.Collections.Concurrent;
using System.Threading;
using SFA.DAS.Payments.Application.Infrastructure.Logging;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing
{
    public class JobStatusManager
    {
        private readonly IPaymentLogger logger;
        private readonly ConcurrentDictionary<long, JobStatusService> jobStatusServices;
        private readonly Timer statusTimer;

        public JobStatusManager(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            jobStatusServices = new ConcurrentDictionary<long, JobStatusService>();
            statusTimer  = new Timer(new TimerCallback(CheckJobsStatus),null,TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
        }

        public void StartMonitoringJob(long jobId)
        {
        }

        private void CheckJobsStatus(object state)
        {

        }
    }
}