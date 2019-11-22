using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using Newtonsoft.Json;

namespace SFA.DAS.Payments.DataLocks.DataLockStatusChangedService.Handlers
{
    public class DataLockEventHandler : IHandleMessages<DataLockEvent>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IReceivedDataLockEventStore receivedDataLockEventStore;


        public DataLockEventHandler(IPaymentLogger paymentLogger, IReceivedDataLockEventStore receivedDataLockEventStore)
        {
            this.paymentLogger = paymentLogger;
            this.receivedDataLockEventStore = receivedDataLockEventStore;
        }

        public async Task Handle(DataLockEvent message, IMessageHandlerContext context)
        {
            paymentLogger.LogVerbose($"Processing {message.GetType().Name} event for UKPRN {message.Ukprn}");

            await receivedDataLockEventStore.Add(new ReceivedDataLockEvent
            {
                JobId = message.JobId,
                Ukprn = message.Ukprn,
                Message = JsonConvert.SerializeObject(message),
                MessageType = message.GetType().AssemblyQualifiedName
            });

            paymentLogger.LogVerbose($"Successfully Processed {message.GetType().Name} event for UKPRN {message.Ukprn}");
        }
    }
}
