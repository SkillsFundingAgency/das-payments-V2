using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESFA.DC.Data.AppsEarningsHistory.Model;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.Core.Services;
using SFA.DAS.Payments.Tests.Core;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.LearnerMutators {
    public class ApprenticeshipEarningsHistoryService : IApprenticeshipEarningsHistoryService
    {
        private readonly AppEarnHistoryContext appEarnHistoryContext;

        public ApprenticeshipEarningsHistoryService(AppEarnHistoryContext appEarnHistoryContext)
        {
            this.appEarnHistoryContext = appEarnHistoryContext ?? throw new ArgumentNullException(nameof(appEarnHistoryContext));
        }

        public async Task AddHistoryAsync(IEnumerable<Learner> learners)
        {
            foreach (var learner in learners)
            {
                foreach (var learnerAim in learner.Aims.Where(a=>a.IsMainAim))
                {
                    var aeh = new AppsEarningsHistory
                              {
                                  AppIdentifier = AppIdentifier(learnerAim),
                                  AppProgCompletedInTheYearInput = learnerAim.CompletionStatus == CompletionStatus.Completed,
                                  BalancingProgAimPaymentsInTheYear = learner.EarningsHistory.BalancingEarningsToDate,
                                  CollectionYear = learner.EarningsHistory.CollectionYear,
                                  CollectionReturnCode = $"R{learner.EarningsHistory.CollectionPeriod}",
                                  CompletionProgaimPaymentsInTheYear = learner.EarningsHistory.CompletionEarningsToDate,
                                  DaysInYear = learner.EarningsHistory.TrainingDaysCompleted(OriginalStartDate(learnerAim)),
                                  FworkCode = learnerAim.ProgrammeType == 25 ? default(int?) : learnerAim.FrameworkCode,
                                  HistoricEffectiveTnpstartDateInput = OriginalStartDate(learnerAim).ToDate(),
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
                                  OnProgProgAimPaymentsInTheYear = learner.EarningsHistory.OnProgrammeEarningsToDate,
                                  ProgrammeStartDateIgnorePathway = OriginalStartDate(learnerAim).ToDate(),
                                  ProgrammeStartDateMatchPathway = OriginalStartDate(learnerAim).ToDate(),
                                  ProgType = learnerAim.ProgrammeType,
                                  PwayCode = learnerAim.ProgrammeType == 25 ? default(int?) : learnerAim.PathwayCode,
                                  Stdcode = learnerAim.ProgrammeType == 25 ? learnerAim.StandardCode : default(int?),
                                  TotalProgAimPaymentsInTheYear = learner.EarningsHistory.TotalEarningsToDate,
                                  UptoEndDate = learner.EarningsHistory.UpToEndDate(OriginalStartDate(learnerAim)),
                                  Ukprn = (int) learner.Ukprn,
                                  Uln = learner.Uln,
                                  HistoricEmpIdEndWithinYear = learner.EarningsHistory.EmployerId,
                                  HistoricEmpIdStartWithinYear = learner.EarningsHistory.EmployerId,
                                  HistoricPmramount = 0,
                                  HistoricLearnDelProgEarliestAct2dateInput = OriginalStartDate(learnerAim).ToDate()
                              };
                    appEarnHistoryContext.AppsEarningsHistories.Add(aeh);
                }
            }
            await appEarnHistoryContext.SaveChangesAsync();
        }

        public void DeleteHistory(long ukprn)
        {
            appEarnHistoryContext.Database.ExecuteSqlCommand(@"delete from dbo.AppsEarningsHistory where UKPRN = {0}", ukprn);
        }

        public async Task DeleteHistoryAsync(long ukprn)
        {
            await appEarnHistoryContext.Database.ExecuteSqlCommandAsync(@"delete from dbo.AppsEarningsHistory where UKPRN = {0}", ukprn);
        }

        private string AppIdentifier(Aim aim)
        {
            return aim.ProgrammeType == 25 
                       ? $"{aim.ProgrammeType}-{aim.StandardCode}" 
                       : $"{aim.ProgrammeType}-{aim.FrameworkCode}-{aim.PathwayCode}";
        }

        private string OriginalStartDate(Aim aim)
        {
            return string.IsNullOrWhiteSpace(aim.OriginalStartDate) ? aim.StartDate : aim.OriginalStartDate;
        }
    }
}