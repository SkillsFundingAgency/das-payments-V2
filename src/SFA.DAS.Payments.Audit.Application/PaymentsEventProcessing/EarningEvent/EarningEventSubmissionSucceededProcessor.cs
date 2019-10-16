using System;
using System.Threading.Tasks;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.EarningEvent
{
    public interface IEarningEventSubmissionSucceededProcessor
    {
        Task Process(SubmissionSucceededEvent message);
    }

    public class EarningEventSubmissionSucceededProcessor: IEarningEventSubmissionSucceededProcessor
    {
        public Task Process(SubmissionSucceededEvent message)
        {
            throw new NotImplementedException();
        }
    }
}