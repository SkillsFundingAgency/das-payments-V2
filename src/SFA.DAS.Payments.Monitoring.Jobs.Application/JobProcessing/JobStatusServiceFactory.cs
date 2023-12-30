using System;
using SFA.DAS.Payments.Application.Infrastructure.UnitOfWork;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.Earnings;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing
{

    public interface IJobStatusServiceFactory
    {
        IJobStatusService Create(IUnitOfWorkScope scope, JobType jobType);
    }

    public class JobStatusServiceFactory: IJobStatusServiceFactory
    {
        public IJobStatusService Create(IUnitOfWorkScope scope, JobType jobType)
        {
            switch (jobType)
            {
                case JobType.EarningsJob:
                case JobType.ComponentAcceptanceTestEarningsJob:
                    return scope.Resolve<IEarningsJobStatusService>();
                case JobType.PeriodEndStartJob:
                    return scope.Resolve<IPeriodEndStartJobStatusService>();
                case JobType.PeriodEndIlrReprocessingJob:
                    return scope.Resolve<IIlrReprocessingJobStatusService>();
                case JobType.PeriodEndRunJob:
                case JobType.PeriodEndStopJob:
                case JobType.ComponentAcceptanceTestMonthEndJob:
                    return scope.Resolve<IPeriodEndJobStatusService>();
                //SubmissionWindow and Reports jobs are handled using a different pattern. The message handlers process those types in-line.
                default:
                    throw new InvalidOperationException(
                        $"Unable to resolve job status service for job type: {jobType}");
            }
        }
    }
}