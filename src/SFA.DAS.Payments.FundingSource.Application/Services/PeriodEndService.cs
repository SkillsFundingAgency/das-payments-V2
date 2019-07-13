using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Messages.Internal.Commands;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Services
{
    public class PeriodEndService: IPeriodEndService
    {
        private readonly ILevyFundingSourceRepository repository;
        private readonly IPaymentLogger logger;

        public PeriodEndService(ILevyFundingSourceRepository repository, IPaymentLogger logger)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<ProcessLevyPaymentsOnMonthEndCommand>> GenerateEmployerPeriodEndCommands(PeriodEndRunningEvent message)
        {
            logger.LogInfo($"Sending requests to trigger month end for job: {message.JobId}");
            var employerPeriodEndCommands = new List<ProcessLevyPaymentsOnMonthEndCommand>();
            var accounts = await repository.GetEmployerAccounts(CancellationToken.None).ConfigureAwait(false);
            foreach (var account in accounts)
            {
                logger.LogDebug($"Triggering period end for account: {account}");
                employerPeriodEndCommands.Add(new ProcessLevyPaymentsOnMonthEndCommand
                {
                    AccountId = account,
                    CollectionPeriod = message.CollectionPeriod,
                    JobId = message.JobId,
                });
            }
            logger.LogInfo($"Finished sending requests to trigger month end for job: {message.JobId}");
            return employerPeriodEndCommands;
        }
    }
}