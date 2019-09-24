using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.JobService.Interfaces;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.Jobs.JobsProxyService.Handlers
{
    public class RecordEarningsJobHandler: BaseJobMessageHandler<RecordEarningsJob>
    {
        public RecordEarningsJobHandler(IServiceProxyFactory proxyFactory, IPaymentLogger logger): base(proxyFactory,logger)
        {
        }

        protected override async Task HandleMessage(RecordEarningsJob message, IMessageHandlerContext context, IJobService jobService,
            CancellationToken cancellationToken)
        {
            //await jobService.RecordEarningsJob(message, cancellationToken).ConfigureAwait(false);
        }
    }
}