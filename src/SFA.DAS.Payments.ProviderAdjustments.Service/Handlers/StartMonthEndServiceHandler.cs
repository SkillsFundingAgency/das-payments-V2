using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;
using SFA.DAS.Payments.ProviderAdjustments.Application;

namespace SFA.DAS.Payments.ProviderAdjustments.Service.Handlers
{
    class StartMonthEndServiceHandler : IHandleMessages<PeriodEndStartedEvent>
    {
        private readonly IProviderAdjustmentsProcessor processor;

        public StartMonthEndServiceHandler(IProviderAdjustmentsProcessor processor)
        {
            this.processor = processor;
        }

        public async Task Handle(PeriodEndStartedEvent message, IMessageHandlerContext context)
        {
            await processor.ProcessEasAtMonthEnd(message.CollectionPeriod.AcademicYear, message.CollectionPeriod.Period);
        }
    }
}
