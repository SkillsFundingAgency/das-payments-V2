using SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Data;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Steps
{
    [Binding]
    public class CourseSteps
    {
        private readonly ScenarioContext scenarioContext;

        public CourseSteps(ScenarioContext context)
        {
            scenarioContext = context;
        }

        [Given(@"the following course information:")]
        public void GivenTheFollowingCourseInformation(Table table)
        {
            scenarioContext["Courses"] = table.CreateSet<Course>();
        }
    }
}