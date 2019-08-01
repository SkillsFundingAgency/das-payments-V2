using System;
using ESFA.DC.Data.AppsEarningsHistory.Model;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Services.Interfaces;

namespace SFA.DAS.Payments.AcceptanceTests.Services.Services
{
    public class ApprenticeshipEarningsHistoryService : IApprenticeshipEarningsHistoryService
    {
        private readonly AppEarnHistoryContext appEarnHistoryContext;

        public ApprenticeshipEarningsHistoryService(AppEarnHistoryContext appEarnHistoryContext)
        {
            this.appEarnHistoryContext = appEarnHistoryContext ?? throw new ArgumentNullException(nameof(appEarnHistoryContext));
        }
        public void DeleteHistory(long ukprn)
        {
            appEarnHistoryContext.Database.ExecuteSqlCommand(@"delete from dbo.AppsEarningsHistory where UKPRN = {0}", ukprn);
        }
    }
}
