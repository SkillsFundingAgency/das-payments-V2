﻿using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.RequiredPayments.Messages.Events
{
    public abstract class ApprenticeshipContractTypeRequiredPaymentEvent : RequiredPaymentEvent
    {
        public decimal SfaContributionPercentage { get; set; }
        public OnProgrammeEarningType OnProgrammeEarningType { get; set; }
    }
}