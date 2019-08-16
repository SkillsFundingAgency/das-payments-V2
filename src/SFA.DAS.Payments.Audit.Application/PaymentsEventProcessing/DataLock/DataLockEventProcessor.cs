using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SFA.DAS.Payments.Audit.Application.PaymentsEventModelCache;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.DataLocks.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.DataLock
{
    public interface IDataLockEventProcessor
    {
        Task ProcessPaymentsEvent(DataLockEvent message, CancellationToken cancellationToken);
    }
    public class DataLockEventProcessor : PaymentsEventProcessor<DataLockEvent, DataLockEventModel>, IDataLockEventProcessor
    {
        public DataLockEventProcessor(IPaymentsEventModelCache<DataLockEventModel> cache, IMapper mapper) : base(cache, mapper)
        {
        }
    }
}



