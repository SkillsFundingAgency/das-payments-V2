using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;
using Newtonsoft.Json;
using SFA.DAS.Payments.DataLocks.Application.Services;

namespace SFA.DAS.Payments.DataLocks.DataLockStatusChangedService.Handlers
{
    public class DataLockEventHandler : IHandleMessages<DataLockEvent>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IManageReceivedDataLockEvent manageReceivedDataLockEvent;
        
        public DataLockEventHandler(IPaymentLogger paymentLogger, IManageReceivedDataLockEvent manageReceivedDataLockEvent)
        {
            this.paymentLogger = paymentLogger;
            this.manageReceivedDataLockEvent = manageReceivedDataLockEvent;
        }

        public async Task Handle(DataLockEvent message, IMessageHandlerContext context)
        {
            paymentLogger.LogVerbose($"Processing {message.GetType().Name} event for UKPRN {message.Ukprn}");
            await manageReceivedDataLockEvent.ProcessDataLockEvent(message);
            paymentLogger.LogVerbose($"Successfully Processed {message.GetType().Name} event for UKPRN {message.Ukprn}");
        }
    }
}
