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
    public class StopPeriodEnd
    {
        [Test]
        public async Task SendPeriodEndStoppedEvent_ApprovalsServiceIsReinstalled()
        {
            var messageSession = await MessageSessionBuilder.BuildAsync();

            var serviceStatus = await ServiceFabricManager.IsApprovalsServiceRunning();
            serviceStatus.Should().BeFalse("Approvals service is running prior to test start");

            var periodEndEvent = new PeriodEndStoppedEvent()
            {
                CollectionPeriod = new CollectionPeriod
                {
                    Period = 1,
                    AcademicYear = 1920
                },
                JobId = 1234,

            };
            await messageSession.Send(periodEndEvent).ConfigureAwait(false);

            var stopTime = DateTime.Now.AddSeconds(15);
            while (DateTime.Now < stopTime)
            {
                serviceStatus = await ServiceFabricManager.IsApprovalsServiceRunning();
                if (serviceStatus)
                {
                    return;
                }

                await Task.Delay(50).ConfigureAwait(false);
            }

            Assert.Fail("Approvals service is not running 15s after sending message");
        }
    }
}
