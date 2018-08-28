using System.Linq;
using SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data;
using SFA.DAS.Payments.Tests.Core.Application;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Steps
{
    [Binding]
    public class CourseSteps
    {
        private readonly ScenarioContext scenarioContext;

        private readonly LearnRefNumberGenerator learnRefNumberGenerator;

        public CourseSteps(ScenarioContext context, LearnRefNumberGenerator generator)
        {
            scenarioContext = context;
            learnRefNumberGenerator = generator;
        }

        [Given(@"the following course information:")]
        public void GivenTheFollowingCourseInformation(Table table)
        {
            var courses = table.CreateSet<Course>().ToList();

            courses.ForEach(c => learnRefNumberGenerator.Generate(c.Ukprn, c.LearnRefNumber));

            scenarioContext["Courses"] = courses;

        }
    }
}