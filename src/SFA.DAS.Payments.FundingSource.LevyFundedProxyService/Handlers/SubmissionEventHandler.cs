using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.FundingSource.LevyFundedService;
using SFA.DAS.Payments.FundingSource.LevyFundedService.Interfaces;
using SFA.DAS.Payments.FundingSource.Messages.Internal.Commands;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core.Factories;

namespace SFA.DAS.Payments.FundingSource.LevyFundedProxyService.Handlers
{
    public abstract class SubmissionEventHandler<T> : IHandleMessages<T> where T : SubmissionEvent
    {
        private readonly IActorProxyFactory proxyFactory;
        private readonly ILevyFundingSourceRepository repository;
        private readonly IPaymentLogger logger;
        private readonly IExecutionContext executionContext;
        private readonly ILevyMessageRoutingService levyMessageRoutingService;

        protected SubmissionEventHandler(IActorProxyFactory proxyFactory, ILevyFundingSourceRepository repository,
            IPaymentLogger logger, IExecutionContext executionContext, ILevyMessageRoutingService levyMessageRoutingService)
        {
            this.proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.executionContext = executionContext ?? throw new ArgumentNullException(nameof(executionContext));
            this.levyMessageRoutingService = levyMessageRoutingService ?? throw new ArgumentNullException(nameof(levyMessageRoutingService));
        }

        public async Task Handle(T message, IMessageHandlerContext context)
        {
            var messageType = message.GetType().Name;
            logger.LogInfo($"Processing {messageType} event. Ukprn: {message.Ukprn}");
            ((ESFA.DC.Logging.ExecutionContext)executionContext).JobId = message.JobId.ToString();

            if (message.Ukprn == 0)
                throw new ArgumentException($"Ukprn cannot be 0. Job Id: {message.JobId}");

            try
            {
                logger.LogDebug($"Getting AccountId for Ukprn: {message.Ukprn}.");
                var accountIds = await repository.GetEmployerAccountsByUkprn(message.Ukprn).ConfigureAwait(false);
                var tasks = new List<Task>();
                foreach (var account in accountIds)
                {
                    var accountToUse = levyMessageRoutingService.GetDestinationAccountId(account.Item1, account.Item2);
                    tasks.Add(InvokeSubmissionAction(accountToUse, message));
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);
                logger.LogInfo($"Successfully processed {messageType} for Job: {message.JobId}, UKPRN: {message.Ukprn}. Skipped submission removing as no account ID found.");
            }
            catch (Exception ex)
            {
                logger.LogError($"Error while handling {messageType} event. Error: {ex.Message}, Job: {message.JobId}, UKPRN: {message.Ukprn}", ex);
                throw;
            }
        }

        private async Task InvokeSubmissionAction(long accountId, T message)
        {
            var actorId = new ActorId(accountId);
            var uri = new Uri(LevyFundedServiceConstants.ServiceUri);
            var actor = proxyFactory.CreateActorProxy<ILevyFundedService>(uri, actorId);

            await HandleSubmissionEvent(message, actor);
            logger.LogInfo($"Successfully processed {typeof(T).Name} event for Actor Id {actorId}, Job: {message.JobId}, UKPRN: {message.Ukprn}");
        }

        protected abstract Task HandleSubmissionEvent(T message, ILevyFundedService fundingService);
    }
}