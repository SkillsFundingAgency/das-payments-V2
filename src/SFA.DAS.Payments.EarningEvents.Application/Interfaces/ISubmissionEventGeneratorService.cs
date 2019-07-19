using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.EarningEvents.Messages.Events;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.EarningEvents.Application.Interfaces
{
    public interface ISubmissionEventGeneratorService
    {
        Task ProcessEarningEvent(IContractTypeEarningEvent earningEvent, CancellationToken cancellationToken);
    }

    public class SubmissionEventGeneratorService : ISubmissionEventGeneratorService
    {
        public Task ProcessEarningEvent(IContractTypeEarningEvent earningEvent, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
