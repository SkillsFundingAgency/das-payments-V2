using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.ProviderAdjustments.Application;

namespace SFA.DAS.Payments.ProviderAdjustments.Service.Handlers
{
    class StartMonthEndServiceHandler : IHandleMessages<StartMonthEndServiceHandler>
    {
        private readonly IProviderAdjustmentsProcessor processor;

        public StartMonthEndServiceHandler(IProviderAdjustmentsProcessor processor)
        {
            this.processor = processor;
        }

        public async Task Handle(StartMonthEndServiceHandler message, IMessageHandlerContext context)
        {
            await processor.ProcessEasAtMonthEnd(1, 1);
        }
    }
}
