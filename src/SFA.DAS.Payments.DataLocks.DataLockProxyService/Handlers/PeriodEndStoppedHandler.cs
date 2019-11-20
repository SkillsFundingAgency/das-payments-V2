using System;
using System.Fabric;
using System.Fabric.Description;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.Handlers
{
    public class PeriodEndStoppedHandler: IHandleMessages<PeriodEndStoppedEvent>
    {
        private readonly IPaymentLogger logger;

        public PeriodEndStoppedHandler(IPaymentLogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PeriodEndStoppedEvent message, IMessageHandlerContext context)
        {
            logger.LogInfo($"Received period end started event. Details: {message.ToJson()}");

            var serviceName = "fabric:/SFA.DAS.Payments.DataLocks.ServiceFabric/SFA.DAS.Payments.DataLocks.ApprovalsService";
            
            var fabricClient = new FabricClient();
            var serviceDescription = new StatelessServiceDescription
            {
                ApplicationName = new Uri("fabric:/SFA.DAS.Payments.DataLocks.ServiceFabric"),
                PartitionSchemeDescription = new SingletonPartitionSchemeDescription(),
                ServiceName = new Uri(serviceName),
                ServiceTypeName = "SFA.DAS.Payments.DataLocks.ApprovalsServiceType",
            };

            try
            {
                await fabricClient.ServiceManager.CreateServiceAsync(serviceDescription);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to create the new service. {e}");
            }
        }
    }
}