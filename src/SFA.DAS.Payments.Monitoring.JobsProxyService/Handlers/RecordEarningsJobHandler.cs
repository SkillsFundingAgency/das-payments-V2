using System;
using System.Threading.Tasks;
using ESFA.DC.Logging.Interfaces;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands;

namespace SFA.DAS.Payments.Monitoring.JobsProxyService.Handlers
{
    public class RecordEarningsJobHandler: IHandleMessages<RecordEarningsJob>
    {
        private readonly IActorProxyFactory proxyFactory;
        private readonly IPaymentLogger logger;

        public RecordEarningsJobHandler(IActorProxyFactory proxyFactory,
            IPaymentLogger logger)
        {
            this.proxyFactory = proxyFactory ?? throw new ArgumentNullException(nameof(proxyFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(RecordEarningsJob message, IMessageHandlerContext context)
        {
            try
            {
                logger.LogVerbose($"Getting actor for job: {message.JobId}");
                var actorId = new ActorId(message.JobId.ToString());
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Failed to record earnings job. ukprn: {message.Ukprn}, Job id: {message.JobId}, Period: {message.CollectionPeriod}-{message.CollectionYear}  Error: {ex.Message}",ex);
                throw;
            }
        }
    }
}