﻿using SFA.DAS.Payments.Model.Core.Incentives;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    public class IncentiveRequiredPayment: RequiredPaymentEvent
    {
        public IncentiveType Type { get; set; }
    }
}