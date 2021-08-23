using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.Model.Core.Entities;
using TechTalk.SpecFlow;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    [Binding]
    [Scope(Feature = "PV2-2635-LearnStartDate-IsMapped-FromILR-ToPaymentsTable")]
    // ReSharper disable once InconsistentNaming
    public class PV2_2635_Steps : FM36_ILR_Base_Steps
    {
        public PV2_2635_Steps(FeatureContext context) : base(context)
        {
        }

        [Then(@"the correct LearnStartDate is set for each generated payment")]
        public void ThenTheCorrectLearnStartDateIsSetForEachGeneratedPayment()
        {
        }

    }
}