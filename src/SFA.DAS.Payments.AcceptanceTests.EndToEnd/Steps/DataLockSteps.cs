using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.EventMatchers;
using SFA.DAS.Payments.DataLocks.Messages.Events;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.OnProgramme;
using SFA.DAS.Payments.Tests.Core;
using SFA.DAS.Payments.Tests.Core.Builders;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Steps
{
    public class DataLockSteps : EndToEndSteps
    {
        public DataLockSteps(FeatureContext context) : base(context)
        {
        }

        [Then(@"the following data lock failures will be generated")]
        public async Task ThenOnlyTheFollowingNonPayableEarningsWillBeGenerated(Table table)
        {
            var dataLockErrors = table.CreateSet<DataLockError>().ToList();
            var matcher = new EarningFailedDataLockMatcher(TestSession.Provider, TestSession, CurrentCollectionPeriod,dataLockErrors);
            await WaitForIt(() => matcher.MatchPayments(), "DataLock Failed event check failure");
        }
    }

   
}
