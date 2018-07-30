using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Payments.Messages.Core;
using SFA.DAS.Payments.PaymentsDue.Messages.Events;

namespace SFA.DAS.Payments.PaymentsDue.Messages.Commands
{
    public class RaisePaymentDueCalculatedEventCommand : PaymentsCommand
    {
        // TODO: NSB gateway through constructor

        Task Execute(ICalculatedPaymentDueEvent calculatedPaymentDueEvent)
        {
            // TODO: call NSB gateway

            return Task.FromResult(0);
        }
    }
}
