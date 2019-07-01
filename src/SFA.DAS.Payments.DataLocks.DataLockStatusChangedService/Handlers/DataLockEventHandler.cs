using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockStatusChangedService.Handlers
{
    public class DataLockEventHandler : IHandleMessages<DataLockEvent>
    {
        private readonly IPaymentLogger paymentLogger;

        public DataLockEventHandler(IPaymentLogger paymentLogger)
        {
            this.paymentLogger = paymentLogger;
        }

        public Task Handle(DataLockEvent message, IMessageHandlerContext context)
        {
            paymentLogger.LogDebug($"Processing {message.GetType().Name} event for UKPRN {message.Ukprn}");
            return Task.CompletedTask;
        }
    }
}
