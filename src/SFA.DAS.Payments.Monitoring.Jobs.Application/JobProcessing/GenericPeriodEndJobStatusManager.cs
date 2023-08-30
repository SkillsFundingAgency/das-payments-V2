using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Management.DataFactory;
using Microsoft.Azure.Management.DataFactory.Models;
using Microsoft.Identity.Client;
using Microsoft.Rest;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Infrastructure.UnitOfWork;
using SFA.DAS.Payments.Monitoring.Jobs.Application.Infrastructure.Configuration;
using SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing.PeriodEnd;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.Monitoring.Jobs.Application.JobProcessing
{
    public interface IGenericPeriodEndJobStatusManager : IJobStatusManager
    {
    }

    public class GenericPeriodEndJobStatusManager : JobStatusManager, IGenericPeriodEndJobStatusManager
    {
        public GenericPeriodEndJobStatusManager(IPaymentLogger logger, IUnitOfWorkScopeFactory scopeFactory,
            IJobServiceConfiguration configuration) : base(logger, scopeFactory, configuration)
        {
        }

        public override IJobStatusService GetJobStatusService(IUnitOfWorkScope scope)
        {
            return scope.Resolve<IGenericPeriodEndJobStatusService>();
        }

        public override async Task<List<long>> GetCurrentJobs(IJobStorageService jobStorage)
        {
            return await jobStorage.GetCurrentPeriodEndExcludingStartJobs(cancellationToken);
        }
    }
}