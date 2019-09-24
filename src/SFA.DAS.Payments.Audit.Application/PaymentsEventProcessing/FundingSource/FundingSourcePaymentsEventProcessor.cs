using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.Audit.Application.PaymentsEventModelCache;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.FundingSource
{
    public interface IFundingSourcePaymentsEventProcessor
    {
        Task ProcessPaymentsEvent(FundingSourcePaymentEvent message, CancellationToken cancellationToken);
    }

    public class FundingSourcePaymentsEventProcessor : PaymentsEventProcessor<FundingSourcePaymentEvent, FundingSourceEventModel>, IFundingSourcePaymentsEventProcessor
    {
        public FundingSourcePaymentsEventProcessor(IPaymentsEventModelCache<FundingSourceEventModel> cache, IMapper mapper) : base(cache, mapper)
        {
        }
    }
}