using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.UnitOfWork;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing
{
    public interface IPeriodEndStartJobStatusManager : IPeriodEndJobStatusManager { }
    
    public class PeriodEndStartJobStatusManager : PeriodEndJobStatusManager, IPeriodEndStartJobStatusManager
    {
        public PeriodEndStartJobStatusManager(IPaymentLogger logger, IUnitOfWorkScopeFactory scopeFactory, 
            IJobServiceConfiguration configuration) : base(logger, scopeFactory, configuration)
        {
        }

        public override IJobStatusService GetJobStatusService(IUnitOfWorkScope scope)
        {
            return scope.Resolve<IPeriodEndStartJobStatusService>();
        }
        
        public override async Task<List<long>> GetCurrentJobs(IJobStorageService jobStorage)
        {
            return await jobStorage.GetCurrentPeriodEndStartJobs(cancellationToken);
        }
    }
}
