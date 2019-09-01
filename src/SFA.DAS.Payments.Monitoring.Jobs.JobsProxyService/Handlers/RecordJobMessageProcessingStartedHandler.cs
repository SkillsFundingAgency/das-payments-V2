using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.JobsService.Interfaces;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobsProxyService.Handlers
{
    public class RecordJobMessageProcessingStartedHandler : JobMessageStatusHandler<RecordStartedProcessingJobMessages>
    {
        public RecordJobMessageProcessingStartedHandler(IActorProxyFactory proxyFactory, IPaymentLogger logger) : base(proxyFactory, logger)
        {
        }

        protected override async Task HandleMessage(RecordStartedProcessingJobMessages message, IMessageHandlerContext context, IJobsService actor, CancellationToken cancellationToken)
        {
            await actor.RecordJobMessageProcessingStartedStatus(message, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}