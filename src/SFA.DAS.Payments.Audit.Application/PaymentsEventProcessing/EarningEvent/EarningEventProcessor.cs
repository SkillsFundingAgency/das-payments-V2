using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.Audit.Application.PaymentsEventModelCache;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.EarningEvent
{
    public interface IEarningEventProcessor
    {
        Task ProcessPaymentsEvent(EarningEvents.Messages.Events.EarningEvent message, CancellationToken cancellationToken);
    }

    public class EarningEventProcessor : PaymentsEventProcessor<EarningEvents.Messages.Events.EarningEvent, EarningEventModel>, IEarningEventProcessor
    {
        public EarningEventProcessor(IPaymentsEventModelCache<EarningEventModel> cache, IMapper mapper) : base(cache, mapper)
        {
        }
    }
}