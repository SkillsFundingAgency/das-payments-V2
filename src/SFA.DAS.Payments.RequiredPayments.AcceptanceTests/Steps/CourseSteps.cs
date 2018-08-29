using System.Linq;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class CourseSteps
    {

        [Given(@"the following course information:")]
        public void GivenTheFollowingCourseInformation(Table table)
        {
            var courses = table.CreateSet<Course>().ToList();

            ScenarioContext.Current["Courses"] = courses;

        }
    }
}