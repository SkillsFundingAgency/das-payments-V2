using System.Threading.Tasks;
using FluentAssertions;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Payments.DataLocks.DataLockProxyService.IntegrationTests.TestUtilities;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.IntegrationTests.MonthEnd
{
    [TestFixture]
    public class StopPeriodEnd
    {
        [Test]
        public async Task SendPeriodEndStoppedEvent_ApprovalsServiceIsReinstalled()
        {
            var messageSession = await MessageSessionBuilder.BuildAsync();

            var serviceStatus = await ServiceFabricManager.IsApprovalsServiceRunning();
            serviceStatus.Should().BeFalse("Approvals service is running prior to test start");

            var periodEndEvent = new PeriodEndStoppedEvent();
            await messageSession.Publish(periodEndEvent).ConfigureAwait(false);

            var result = await new TimeService()
                .WaitForBoolean(async () =>
                        await ServiceFabricManager.IsApprovalsServiceRunning(),
                    true);

            if (result)
            {
                return;
            }

            Assert.Fail("Approvals service is not running 15s after sending message");
        }
    }
}
