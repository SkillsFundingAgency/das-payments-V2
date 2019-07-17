using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ESFA.DC.Data.AppsEarningsHistory.Model;
using ESFA.DC.Data.AppsEarningsHistory.Model.Interface;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Tests.Core;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Services
{
    public interface IApprenticeshipEarningsHistoryService
    {
        Task AddHistoryAsync(int collectionYear, byte collectionPeriod, IEnumerable<Learner> learners);
    }

    public class ApprenticeshipEarningsHistoryService : IApprenticeshipEarningsHistoryService
    {
        private readonly AppEarnHistoryContext appEarnHistoryContext;

        public ApprenticeshipEarningsHistoryService(AppEarnHistoryContext appEarnHistoryContext)
        {
            this.appEarnHistoryContext = appEarnHistoryContext ?? throw new ArgumentNullException(nameof(appEarnHistoryContext));
        }

        public async Task AddHistoryAsync(int collectionYear, byte collectionPeriod, IEnumerable<Learner> learners)
        {
            foreach (var learner in learners)
            {
                await appEarnHistoryContext.Database.ExecuteSqlCommandAsync(@"delete from dbo.AppsEarningsHistory where UKPRN = {0}", learner.Ukprn);
               foreach (var learnerAim in learner.Aims)
               {
                   var aeh = new AppsEarningsHistory
                             {
                                 AppIdentifier = $"{learnerAim.ProgrammeType}-{learnerAim.FrameworkCode}-{learnerAim.PathwayCode}",
                                 AppProgCompletedInTheYearInput = false,
                                 BalancingProgAimPaymentsInTheYear = 0,
                                 CollectionYear = collectionYear.ToString(),
                                 CollectionReturnCode = $"R{collectionPeriod}",
                                 CompletionProgaimPaymentsInTheYear = 0,
                                 DaysInYear = 62, //learner.EarningsHistory.TrainingDaysCompleted(), // todo compute
                                 FworkCode = learnerAim.FrameworkCode,
                                 HistoricEffectiveTnpstartDateInput = learnerAim.OriginalStartDate.ToDate(),
                                 HistoricLearner1618StartInput = false,
                                 HistoricTotal1618UpliftPaymentsInTheYearInput = 0,
                                 HistoricTnp1input = learnerAim.PriceEpisodes.Single().TotalTrainingPrice,
                                 HistoricTnp2input = 0,
                                 HistoricTnp3input = 0,
                                 HistoricTnp4input = 0,
                                 HistoricVirtualTnp3endOfTheYearInput = 0,
                                 HistoricVirtualTnp4endOfTheYearInput = 0,
                                 LatestInYear = true,
                                 LearnRefNumber = learner.LearnRefNumber,
                                 OnProgProgAimPaymentsInTheYear = 2000,//learner.EarningsHistory.OnProgrammeEarningsToDate(),  // todo compute
                                 ProgrammeStartDateIgnorePathway = learnerAim.OriginalStartDate.ToDate(),
                                 ProgrammeStartDateMatchPathway = learnerAim.OriginalStartDate.ToDate(),
                                 ProgType = learnerAim.ProgrammeType,
                                 PwayCode = learnerAim.PathwayCode,
                                 Stdcode = null,
                                 TotalProgAimPaymentsInTheYear = 2000, //learner.EarningsHistory.OnProgrammeEarningsToDate(), //todo compute
                                 UptoEndDate = new DateTime(2018, 11, 1), //learner.EarningsHistory.UpToEndDate, //todo compute
                                 Ukprn = (int)learner.Ukprn,
                                 Uln = learner.Uln,
                                 HistoricEmpIdEndWithinYear = 154549452,//learner.EarningsHistory.Employer, //todo dynamic update
                                 HistoricEmpIdStartWithinYear = 154549452,//learner.EarningsHistory.Employer,
                                 HistoricPmramount = 0,
                                 HistoricLearnDelProgEarliestAct2dateInput = learnerAim.StartDate.ToDate(),
                             };
                   appEarnHistoryContext.AppsEarningsHistories.Add(aeh);
               }
            }
            await appEarnHistoryContext.SaveChangesAsync();
        }
    }
}
