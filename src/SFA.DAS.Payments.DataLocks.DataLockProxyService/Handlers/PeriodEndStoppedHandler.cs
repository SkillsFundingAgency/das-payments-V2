using System;
using System.Fabric;
using System.Fabric.Description;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Core;
using SFA.DAS.Payments.Core.Configuration;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.Handlers
{
    public class PeriodEndStoppedHandler: IHandleMessages<PeriodEndStoppedEvent>
    {
        private readonly IPaymentLogger logger;
        private readonly int instanceCount = -1;

        public PeriodEndStoppedHandler(IPaymentLogger logger, IConfigurationHelper configuration)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var setting = configuration.GetSetting("ApprovalsService_InstanceCount");
            if (!string.IsNullOrWhiteSpace(setting) && int.TryParse(setting, out var result))
            {
                instanceCount = result;
            }
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
                InstanceCount = instanceCount,
            };

            await fabricClient.ServiceManager.CreateServiceAsync(serviceDescription);
        }
    }
}