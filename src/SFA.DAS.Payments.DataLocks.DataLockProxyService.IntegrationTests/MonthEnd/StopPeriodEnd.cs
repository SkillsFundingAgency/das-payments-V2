using System;
using System.Threading.Tasks;
using FluentAssertions;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.IntegrationTests.MonthEnd
{
    public class StopPeriodEnd
    {
#if DEBUG
        [Test]
        public async Task SendPeriodEndStoppedEvent()
        {
            var messageSession = await MessageSessionBuilder.BuildAsync();

            var serviceStatus = await ServiceFabricManager.IsApprovalsServiceRunning();
            serviceStatus.Should().BeFalse("Approvals service is running prior to test start");

            await messageSession.Publish<PeriodEndStoppedEvent>(m => {}, new PublishOptions());

            var stopTime = DateTime.Now.AddSeconds(15);
            while (DateTime.Now < stopTime)
            {
                serviceStatus = await ServiceFabricManager.IsApprovalsServiceRunning();
                if (serviceStatus)
                {
                    return;
                }
            }

            Assert.Fail("Approvals service is not running 15s after sending message");
        }
#endif
    }
}
