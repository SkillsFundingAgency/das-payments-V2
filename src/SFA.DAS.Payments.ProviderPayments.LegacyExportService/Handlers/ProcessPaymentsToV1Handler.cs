using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;

namespace SFA.DAS.Payments.ProviderPayments.LegacyExportService.Handlers
{
    public class ProcessPaymentsToV1Handler : IHandleMessages<ProcessProviderMonthEndCommand>
    {
        public IPaymentExportService PaymentExportService { get; set; }
        
        public async Task Handle(ProcessProviderMonthEndCommand message, IMessageHandlerContext context)
        {
            await PaymentExportService.PerformExportPaymentsToV1(message.CollectionPeriod).ConfigureAwait(false);
            await PaymentExportService.PerformExportEarningsToV1(message.CollectionPeriod).ConfigureAwait(false);
            await context.SendLocal(new ProcessTriggerMonthComplete(message.CollectionPeriod)).ConfigureAwait(false);
        }
    }
}
