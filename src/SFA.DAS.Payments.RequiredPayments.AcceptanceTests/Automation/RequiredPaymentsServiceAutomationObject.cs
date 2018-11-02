using System;
using System.Threading.Tasks;
using NServiceBus;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Automation
{
    public class RequiredPaymentsServiceAutomationObject
    {
        private readonly IMessageSession messageSession;

        public RequiredPaymentsServiceAutomationObject(IMessageSession messageSession)
        {
            this.messageSession = messageSession ?? throw new ArgumentNullException(nameof(messageSession));
        }

        public Task SendPaymentsDue()
        {
            return Task.FromResult(0);
        }
        
    }
}