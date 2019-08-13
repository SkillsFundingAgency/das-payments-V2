using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public class ProcessPaymentsToV1Handler : IHandleMessages<ProcessProviderMonthEndCommand>
    {
        public IPaymentExportService PaymentExportTest { get; set; }
        
        public async Task Handle(ProcessProviderMonthEndCommand message, IMessageHandlerContext context)
        {
            await PaymentExportTest.PerformExportPaymentsToV1(message.CollectionPeriod).ConfigureAwait(false);
        }
    }
}
