using System.Threading.Tasks;
using SFA.DAS.Payments.Monitoring.Metrics.Function.AcceptanceTests.Helpers;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.Monitoring.Metrics.Function.AcceptanceTests.Steps
{
    [Binding]
    public class SharedSteps
    {
        [Given(@"we call the api")]
        public async Task GivenWeCallTheApi()
        {
            ValidateSubmissionWindowTriggerHelper.Trigger(1, 2021, 07);
        }
    }
}
