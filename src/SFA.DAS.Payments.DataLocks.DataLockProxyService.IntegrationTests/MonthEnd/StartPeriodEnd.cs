using System;
using System.Threading.Tasks;
using FluentAssertions;
using NServiceBus;
using NUnit.Framework;
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

            var periodEndEvent = new PeriodEndStartedEvent
            {
                CollectionPeriod = new CollectionPeriod
                {
                    Period = 1,
                    AcademicYear = 1920
                },
                JobId = 1234
            };
            await messageSession.Send(periodEndEvent).ConfigureAwait(false);

            var stopTime = DateTime.Now.AddSeconds(15);
            while (DateTime.Now < stopTime)
            {
                serviceStatus = await ServiceFabricManager.IsApprovalsServiceRunning();
                if (serviceStatus == false)
                {
                    return;
                }
                await Task.Delay(50).ConfigureAwait(false);
            }

            Assert.Fail("Approvals service is still running 15s after sending message");
        }
    }
}
