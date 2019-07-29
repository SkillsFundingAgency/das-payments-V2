using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;

namespace SFA.DAS.Payments.FundingSource.LevyFundedProxyService.Handlers
{
    public class PeriodEndHandler: IHandleMessages<PeriodEndRunningEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly IPeriodEndService periodEndService;

        public PeriodEndHandler(IPaymentLogger logger, IPeriodEndService periodEndService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.periodEndService = periodEndService ?? throw new ArgumentNullException(nameof(periodEndService));
        }

        public async Task Handle(PeriodEndRunningEvent message, IMessageHandlerContext context)
        {
            try
            {
                logger.LogInfo($"Received Period End event. Now getting employer period end commands for collection: {message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear:0000}.");
                var employerPeriodEndCommands = await periodEndService.GenerateEmployerPeriodEndCommands(message);
                logger.LogDebug($"Got {employerPeriodEndCommands.Count} employer period end commands.");
                foreach (var processLevyPaymentsOnMonthEndCommand in employerPeriodEndCommands)
                {
                    logger.LogInfo($"Sending period end command for employer '{processLevyPaymentsOnMonthEndCommand.AccountId}'");
                    await context.SendLocal(processLevyPaymentsOnMonthEndCommand).ConfigureAwait(false);
                }
                logger.LogInfo($"Finished sending employer period end commands for collection: {message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear:0000}.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error triggering generation of levy payments for collection: {message.CollectionPeriod.Period:00}-{message.CollectionPeriod.AcademicYear:0000}");
                throw;
            }
        }
    }
}