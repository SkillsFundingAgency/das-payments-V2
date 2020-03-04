using System.Threading.Tasks;
using FluentAssertions;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.DataLockProxyService.IntegrationTests.TestUtilities;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.IntegrationTests.MonthEnd
{
    [TestFixture]
    public class StartPeriodEnd
    {
        [Test]
        public async Task SendPeriodEndStartedEvent_ApprovalsServiceIsRemoved()
        {
            var messageSession = await MessageSessionBuilder.BuildAsync();

            var serviceStatus = await ServiceFabricManager.IsApprovalsServiceRunning();
            serviceStatus.Should().BeTrue("Approvals service is not running prior to test");

            var periodEndEvent = new PeriodEndStartedEvent{CollectionPeriod = new CollectionPeriod{AcademicYear = 1920, Period = 6}};
            await messageSession.Publish(periodEndEvent).ConfigureAwait(false);

            var result = await new TimeService()
                .WaitForBoolean(async () =>
                        await ServiceFabricManager.IsApprovalsServiceRunning(),
                    false);

            if (result == false)
            {
                return;
            }
        
            Assert.Fail("Approvals service is still running 15s after sending message");
        }
    }
}
