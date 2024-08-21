using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Model.Core;
using System.Collections.Generic;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    public interface IFundingSourceHelper
    {
        List<LevyTransactionModel> GetLevyTransactions(long ukprn, CollectionPeriod collectionPeriod);
    }
}
