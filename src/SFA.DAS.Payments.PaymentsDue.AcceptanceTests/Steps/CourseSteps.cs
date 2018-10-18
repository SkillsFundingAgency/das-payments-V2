using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
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
            TestSession.Learner.Course = table.CreateSet<Course>().First();
        }
    }
}
