using System;
using SFA.DAS.Payments.EarningEvents.Messages.Entities;
using SFA.DAS.Payments.Messages.Core;

namespace SFA.DAS.Payments.EarningEvents.Messages.Events
{
    public interface IEarningEvent : IPaymentsEvent
    {
        EarningEntity Earning {get;set;}
    }
}
