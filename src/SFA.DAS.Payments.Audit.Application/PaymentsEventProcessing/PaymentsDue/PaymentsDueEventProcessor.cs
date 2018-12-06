using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.Audit.Application.PaymentsEventModelCache;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.PaymentsDue
{
    public interface IPaymentsDueEventProcessor
    {
        Task ProcessPaymentsEvent(PaymentDueEvent message);
    }

    public class PaymentsDueEventProcessor : PaymentsEventProcessor<PaymentDueEvent, PaymentsDueEventModel>, IPaymentsDueEventProcessor
    {
        public PaymentsDueEventProcessor(IPaymentsEventModelCache<PaymentsDueEventModel> cache, IMapper mapper) : base(cache, mapper)
        {
        }
    }
}