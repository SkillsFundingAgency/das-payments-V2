using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.JobsService.Interfaces;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobsProxyService.Handlers
{
    public class RecordJobMessageProcessingCompletedHandler : JobMessageStatusHandler<RecordJobMessageProcessingStatus>
    {
        public RecordJobMessageProcessingCompletedHandler(IActorProxyFactory proxyFactory,
            IPaymentLogger logger): base(proxyFactory,logger)
        {
        }

        protected override async Task HandleMessage(RecordJobMessageProcessingStatus message, IMessageHandlerContext context, IJobsService actor, CancellationToken cancellationToken)
        {
            var jobStatus = await actor.RecordJobMessageProcessingStatus(message, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}