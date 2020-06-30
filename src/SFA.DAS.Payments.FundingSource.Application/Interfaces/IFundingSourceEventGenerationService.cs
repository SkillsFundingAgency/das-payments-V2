using System.Collections.ObjectModel;
using System.Threading.Tasks;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface IFundingSourceEventGenerationService
    {
        Task<ReadOnlyCollection<FundingSourcePaymentEvent>> HandleMonthEnd(long employerAccountId, long jobId, CollectionPeriod collectionPeriod);
    }
}