using System;
using System.Threading.Tasks;
using FluentAssertions;
using NServiceBus;
using NUnit.Framework;
using SFA.DAS.Payments.PeriodEnd.Messages.Events;

namespace SFA.DAS.Payments.DataLocks.DataLockProxyService.IntegrationTests.MonthEnd
{
    public class StartPeriodEnd
    {
#if DEBUG
        [Test]
        public async Task SendPeriodEndStartedEvent()
        {
            var messageSession = await MessageSessionBuilder.BuildAsync();

            var serviceStatus = await ServiceFabricManager.IsApprovalsServiceRunning();
            serviceStatus.Should().BeTrue("Approvals service is not running prior to test");

            await messageSession.Publish<PeriodEndStartedEvent>(m => { }, new PublishOptions());

            var stopTime = DateTime.Now.AddSeconds(15);
            while (DateTime.Now < stopTime)
            {
                serviceStatus = await ServiceFabricManager.IsApprovalsServiceRunning();
                if (serviceStatus == false)
                {
                    return;
                }
            }

            Assert.Fail("Approvals service is still running 15s after sending message");
        }
#endif
    }
}
