using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.Audit.Application.PaymentsEventModelCache;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.RequiredPayment
{
    public interface IRequiredPaymentEventProcessor
    {
        Task ProcessPaymentsEvent(PeriodisedRequiredPaymentEvent message);
    }

    public class RequiredPaymentEventProcessor : PaymentsEventProcessor<PeriodisedRequiredPaymentEvent, RequiredPaymentEventModel>, IRequiredPaymentEventProcessor
    {
        public RequiredPaymentEventProcessor(IPaymentsEventModelCache<RequiredPaymentEventModel> cache, IMapper mapper) : base(cache, mapper)
        {
        }
    }
}