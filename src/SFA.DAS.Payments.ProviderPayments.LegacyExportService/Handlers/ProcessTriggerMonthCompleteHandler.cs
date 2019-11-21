using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;

namespace SFA.DAS.Payments.ProviderPayments.LegacyExportService.Handlers
{
    class ProcessTriggerMonthCompleteHandler : IHandleMessages<ProcessTriggerMonthComplete>
    {
        public IPaymentExportService PaymentExportService { get; set; }

        public async Task Handle(ProcessTriggerMonthComplete message, IMessageHandlerContext context)
        {
            await PaymentExportService.PerformMonthEndTrigger(message.CollectionPeriod).ConfigureAwait(false);
        }
    }
}
