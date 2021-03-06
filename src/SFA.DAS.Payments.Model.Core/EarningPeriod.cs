﻿using System;
using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Model.Core
{
    public class EarningPeriod
    {
        public string PriceEpisodeIdentifier { get; set; }
        public byte Period { get; set; }
        public decimal Amount { get; set; }
        public decimal? SfaContributionPercentage { get; set; }
        public long? AccountId { get; set; }
        public long? ApprenticeshipId { get; set; }
        public long? ApprenticeshipPriceEpisodeId { get; set; }
        public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
        public long? TransferSenderAccountId { get; set; }
        public int? Priority { get; set; }
        public List<DataLockFailure> DataLockFailures { get; set; }
        public DateTime? AgreedOnDate { get; set; }
    }
}
