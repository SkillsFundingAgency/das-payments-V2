using System;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces;
using SFA.DAS.Payments.FundingSource.Messages.Internal.Commands;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core.Factories;

namespace SFA.DAS.Payments.FundingSource.LevyFundedProxyService.Handlers
{
    public class SubmissionSucceededEventHandler:IHandleMessages<SubmissionSucceededEvent>
    {
        private readonly IActorProxyFactory proxyFactory;
        private readonly ILevyFundingSourceRepository repository;
        private readonly IPaymentLogger logger;
        private readonly IExecutionContext executionContext;

        public SubmissionSucceededEventHandler(IActorProxyFactory proxyFactory, ILevyFundingSourceRepository repository,
            IPaymentLogger logger, IExecutionContext executionContext)
        {
            this.proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
            this.repository = repository?? throw new ArgumentNullException(nameof(repository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.executionContext = executionContext ?? throw new ArgumentNullException(nameof(executionContext));
        }

        public async Task Handle(SubmissionSucceededEvent message, IMessageHandlerContext context)
        {
            logger.LogInfo($"Processing SubmissionSucceededEvent event. Ukprn: {message.Ukprn}");
            ((ESFA.DC.Logging.ExecutionContext)executionContext).JobId = message.JobId.ToString();

            if (message.Ukprn == 0)
                throw new ArgumentException($"Ukprn cannot be 0. Job Id: {message.JobId}");

            try
            {
                logger.LogDebug($"Getting AccountId for Ukprn: {message.Ukprn}.");
                var accountId = await repository.GetLevyAccountId(message.Ukprn).ConfigureAwait(false);
                var actorId = new ActorId(accountId);
                var actor = proxyFactory.CreateActorProxy<ILevyFundedService>(new Uri("fabric:/SFA.DAS.Payments.FundingSource.ServiceFabric/LevyFundedServiceActorService"), actorId);

                var processCommand = new ProcessPreviousSubmissionDeletionCommand
                {
                    AccountId = accountId,
                    CollectionPeriod =
                        CollectionPeriodFactory.CreateFromAcademicYearAndPeriod(message.AcademicYear,
                            message.CollectionPeriod),
                    JobId = message.JobId,
                    CommandId = Guid.NewGuid(),
                    RequestTime = message.EventTime,
                    SubmissionDate = message.IlrSubmissionDateTime
                };

                await actor.RemovePreviousSubmissions(processCommand).ConfigureAwait(false);
                logger.LogInfo($"Successfully processed SubmissionSucceededEvent event for Actor Id {actorId}, Job: {message.JobId}, UKPRN: {message.Ukprn}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error while handling SubmissionSucceededEvent event. Error: {ex.Message}, Job: {message.JobId}, UKPRN: {message.Ukprn}", ex);
                throw;
            }
        }
    }
}