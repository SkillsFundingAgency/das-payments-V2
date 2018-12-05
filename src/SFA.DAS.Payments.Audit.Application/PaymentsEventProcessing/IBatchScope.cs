using System;
using System.Threading.Tasks;
using SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing.FundingSource;
using SFA.DAS.Payments.Audit.Model;

namespace SFA.DAS.Payments.Audit.Application.PaymentsEventProcessing
{
    public interface IBatchScope: IDisposable
    {
        IPaymentsEventModelBatchProcessor<T> GetBatchProcessor<T>() where T: PaymentsEventModel;
        void Abort();
        Task Commit();
    }
}