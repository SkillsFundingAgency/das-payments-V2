using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.Audit.Application.PaymentsEventModelCache;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.FundingSource.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.FundingSource
{
    public interface IFundingSourceEventProcessor
    {
        Task ProcessPaymentsEvent(FundingSourcePaymentEvent message, CancellationToken cancellationToken);
    }

    public class FundingSourceEventProcessor : PaymentsEventProcessor<FundingSourcePaymentEvent, FundingSourceEventModel>, IFundingSourceEventProcessor
    {
        public FundingSourceEventProcessor(IPaymentsEventModelCache<FundingSourceEventModel> cache, IMapper mapper) : base(cache, mapper)
        {
        }
    }
}