using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;

namespace SFA.DAS.Payments.ProviderPayments.LegacyExportService.Handlers
{
    public class ProcessPaymentsToV1Handler : IHandleMessages<PeriodEndStoppedEvent>
    {
        public IPaymentExportService PaymentExportService { get; set; }
        
        public async Task Handle(PeriodEndStoppedEvent message, IMessageHandlerContext context)
        {
            await PaymentExportService.PerformExportPaymentsAndEarningsToV1(message.CollectionPeriod).ConfigureAwait(false);
            await context.SendLocal(new ProcessTriggerMonthComplete(message.CollectionPeriod){JobId = message.JobId}).ConfigureAwait(false);
        }
    }
}
