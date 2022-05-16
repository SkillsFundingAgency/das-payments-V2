using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FPA.Messages.Events;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Messages.Internal.Commands;

namespace SFA.DAS.Payments.FundingSource.LevyFundedProxyService.Handlers
{
    public class FlexiPaymentsReadyForProcessingEventHandler : IHandleMessages<FlexiPaymentsReadyForProcessingEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly ILevyFundingSourceRepository repository;

        public FlexiPaymentsReadyForProcessingEventHandler(IPaymentLogger logger, ILevyFundingSourceRepository repository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.repository = repository;
        }

        public async Task Handle(FlexiPaymentsReadyForProcessingEvent message, IMessageHandlerContext context)
        {
            //todo Generate ProcessLevyPaymentsOnMonthEndCommands for each employer (similar to public async Task<List<ProcessLevyPaymentsOnMonthEndCommand>> GenerateEmployerPeriodEndCommands(PeriodEndRunningEvent message) but only for specified employers)
            var processLevyPaymentsOnMonthEndCommand = await GenerateEmployerPeriodEndCommand(message);

            //todo skip this
            //logger.LogInfo($"Received Period End event. Now getting employer period end commands for collection: {message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear:0000}.");
            //var employerPeriodEndCommands = await periodEndService.GenerateEmployerPeriodEndCommands(message).ConfigureAwait(false);
            //logger.LogDebug($"Got {employerPeriodEndCommands.Count} employer period end commands.");

            //todo then do this with the results
            await context.SendLocal(processLevyPaymentsOnMonthEndCommand).ConfigureAwait(false);

            //logger.LogInfo($"Finished sending employer period end commands for collection: {message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear:0000}.");
            //todo logging if needed for spike
        }

        private async Task<ProcessLevyPaymentsOnMonthEndCommand> GenerateEmployerPeriodEndCommand(
            FlexiPaymentsReadyForProcessingEvent message)
        {
            var accounts = await repository.GetEmployerAccounts(CancellationToken.None).ConfigureAwait(false); //inefficient todo consider writing custom method to just query DB for passed in accounts
            if (!accounts.Contains(message.EmployerAccountId))
                throw new Exception($"Employer account {message.EmployerAccountId} not found in DB.");
            return new ProcessLevyPaymentsOnMonthEndCommand
            {
                AccountId = message.EmployerAccountId,
                CollectionPeriod = message.CollectionPeriod,
                JobId = message.JobId
            };
        }
    }
}