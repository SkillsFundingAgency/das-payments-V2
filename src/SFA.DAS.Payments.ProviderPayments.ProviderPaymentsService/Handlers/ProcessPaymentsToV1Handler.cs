using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Payments.ProviderPayments.Application.Services;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;

namespace SFA.DAS.Payments.ProviderPayments.ProviderPaymentsService.Handlers
{
    public class ProcessPaymentsToV1Handler : IHandleMessages<ProcessProviderMonthEndCommand>
    {
        public IMonthEndService MonthEndService { get; set; }

        public async Task Handle(ProcessProviderMonthEndCommand message, IMessageHandlerContext context)
        {
            var payments = await MonthEndService.GetMonthEndPayments(message.CollectionPeriod, message.Ukprn)
                .ConfigureAwait(false);

            
        }
    }
}
