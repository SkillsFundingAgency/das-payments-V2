using System.Linq;
using NUnit.Framework;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Data;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Steps
{
    [Binding]
    public class CourseSteps : StepsBase
    {
        public CourseSteps(ScenarioContext scenarioContext) : base(scenarioContext)
        {
        }

        [Given(@"planned course duration is (.*) months")]
        public void GivenPlannedCourseDurationIsMonths(int p0)
        {            
        }
        
        [Given(@"the following course information:")]
        public void GivenTheFollowingCourseInformation(Table table)
        {
            var learnerCourse = table.CreateSet<LearnerOnCourse>().First();
            Assert.IsNotNull(learnerCourse);
            TestSession.Learner.Course = learnerCourse;
            TestSession.Learner.LearnerIdentifier = learnerCourse.LearnerId;
        }

        [Given(@"the following course information for Learners:")]
        public void GivenTheFollowingCourseInformationForLearners(Table table)
        {
            TestSession.Learners.Clear();
            var courses = table.CreateSet<LearnerOnCourse>().ToList();
            foreach (var course in courses)
            {
                var learner = TestSession.GenerateLearner();
                learner.LearnerIdentifier = course.LearnerId;
                learner.Course = course;
                TestSession.Learners.Add(learner);
            }
        }
    }
}
