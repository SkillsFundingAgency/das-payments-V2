using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    [Scope(Feature = "PV2-2465-Duplicate-Price-Episode-Bug")]
    // ReSharper disable once InconsistentNaming
    public class PV2_2465_Steps : FM36_ILR_Base_Steps
    {
        public PV2_2465_Steps(FeatureContext context) : base(context)
        {
        }

        [Given("a learner with multiple price episodes and learning aims")]
        public async Task LearnerWithMultiplePriceEpisodesAndLearningAims()
        {
            ImportR07Fm36();

            TestSession.FM36Global.Learners.ForEach(x => x.ULN = TestSession.Learner.Uln);
            TestSession.Learner.LearnRefNumber = TestSession.FM36Global.Learners.First().LearnRefNumber;

            var dcHelper = Scope.Resolve<IDcHelper>();
            await dcHelper.SendIlrSubmission(TestSession.FM36Global.Learners,
                TestSession.Provider.Ukprn,
                TestSession.CollectionPeriod.AcademicYear,
                TestSession.CollectionPeriod.Period,
                TestSession.Provider.JobId);
        }

        [Then(@"the correct course date from the earlier learning aim is set on the earning event")]
        public async Task ThenTheCorrectCourseDateFromTheEarlierLearningAimIsSetOnTheEarningEvent()
        {
            await WaitForIt(() => HasEarningEventWithCorrectPriceEpisodeCourseStartDate(), "Earning event not found with correct course start date");
            await WaitForUnexpected(HasNoEarningEventWithoutCorrectPriceEpisodeCourseStartDate, "Found earning event with incorrect course start date");
        }

        private void ImportR07Fm36() { GetFm36LearnerForCollectionPeriod("R07/current academic year"); }

        private bool HasEarningEventWithCorrectPriceEpisodeCourseStartDate()
        {
            var fm36Year = TestSession.FM36Global.Learners.First().PriceEpisodes.First().PriceEpisodeValues.EpisodeStartDate.Value.Year;

            var events = EarningEventsHelper.EarningEventsReceivedForLearner(TestSession).ToList();
            return EarningEventsHelper.EarningEventsReceivedForLearner(TestSession)
                .Where(x => x.JobId == TestSession.Provider.JobId)
                .Any(x => x.PriceEpisodes.All(y => y.CourseStartDate.Date == new DateTime(fm36Year, 8, 6)));
        }

        private (bool pass, string reason) HasNoEarningEventWithoutCorrectPriceEpisodeCourseStartDate()
        {
            var fm36Year = TestSession.FM36Global.Learners.First().PriceEpisodes.First().PriceEpisodeValues.EpisodeStartDate.Value.Year;

            var events = EarningEventsHelper.EarningEventsReceivedForLearner(TestSession).ToList();
            return !EarningEventsHelper.EarningEventsReceivedForLearner(TestSession)
                .Where(x => x.JobId == TestSession.Provider.JobId)
                .Any(x => x.PriceEpisodes.Any(y => y.CourseStartDate.Date != new DateTime(fm36Year, 8, 6)))
                ? (true, string.Empty)
                : (false, $"Found earning event with price episode course start date not matching expected {fm36Year}-08-06");
        }
    }
}