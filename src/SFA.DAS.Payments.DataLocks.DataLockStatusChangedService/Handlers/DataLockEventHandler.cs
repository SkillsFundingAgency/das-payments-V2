using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.DataLocks.Application.Interfaces;
using SFA.DAS.Payments.DataLocks.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Application.Services;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.DataLockStatusChangedService.Handlers
{
    public class DataLockEventHandler : IHandleMessages<DataLockEvent>
    {
        private readonly IPaymentLogger paymentLogger;
        private readonly IReceivedDataLockEventStore dataLockEventStore;
        private readonly IDataLockEventProcessor dataLockEventProcessor;

        public DataLockEventHandler(IPaymentLogger paymentLogger, IReceivedDataLockEventStore dataLockEventStore )
        {
            this.paymentLogger = paymentLogger;
            this.dataLockEventStore = dataLockEventStore;
            this.dataLockEventProcessor = dataLockEventProcessor;
        }

        public async Task Handle(DataLockEvent message, IMessageHandlerContext context)
        {
            paymentLogger.LogVerbose($"Processing {message.GetType().Name} event for UKPRN {message.Ukprn}");

           //await dataLockEventStore.Add(new ReceivedDataLockEvent
           //{
               
           //})


        }
    }
}
