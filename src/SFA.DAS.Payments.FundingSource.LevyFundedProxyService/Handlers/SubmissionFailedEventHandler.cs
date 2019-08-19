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
    public class SubmissionFailedEventHandler : SubmissionEventHandler, IHandleMessages<SubmissionFailedEvent>
    {
        public SubmissionFailedEventHandler(IActorProxyFactory proxyFactory, ILevyFundingSourceRepository repository,
            IPaymentLogger logger, IExecutionContext executionContext) : base(proxyFactory, repository, logger, executionContext)
        {
        }

        public async Task Handle(SubmissionFailedEvent message, IMessageHandlerContext context)
        {
            await Handle(message);
        }

        protected override async Task RemoveSubmissions(ILevyFundedService fundingService,
            ProcessSubmissionDeletion submission)
        {
            await fundingService.RemoveCurrentSubmission(submission);
        }
    }
}