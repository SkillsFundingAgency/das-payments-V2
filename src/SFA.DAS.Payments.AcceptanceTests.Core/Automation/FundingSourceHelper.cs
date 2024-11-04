using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    public class FundingSourceHelper : IFundingSourceHelper
    {
        private readonly TestFundingSourceDataContext dataContext;

        public FundingSourceHelper(TestFundingSourceDataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public List<LevyTransactionModel> GetLevyTransactions(long ukprn, CollectionPeriod collectionPeriod)
        {
            using (dataContext.Database.BeginTransaction(IsolationLevel.ReadUncommitted))
                return dataContext.LevyTransactions
                    .Where(x => x.Ukprn == ukprn &&
                                x.CollectionPeriod == collectionPeriod.Period &&
                                x.AcademicYear == collectionPeriod.AcademicYear)
                    .ToList();
        }
    }
}
