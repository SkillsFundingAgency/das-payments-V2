﻿using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.FundingSource.Domain.Models
{
    public class RequiredPayment
    {
        public decimal SfaContributionPercentage { get; set; }
        public decimal AmountDue { get; set; }
        public bool IsTransfer { get; set; }
        public FundingPlatformType FundingPlatformType { get; set; }
    }
}