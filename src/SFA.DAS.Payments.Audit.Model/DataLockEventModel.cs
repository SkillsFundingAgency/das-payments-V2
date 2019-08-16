﻿using System;
using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.Audit.Model
{
    public class DataLockEventModel : PaymentsEventModel
    {
        public Guid EarningEventId { get; set; }
        public List<PriceEpisode> PriceEpisodes { get; set; }
        public short CollectionYear { get; set; }
        public string AgreementId { get; set; }
        public List<OnProgrammeEarning> OnProgrammeEarnings { get; set; }
        public List<IncentiveEarning> IncentiveEarnings { get; set; }
        public bool IsPayable { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
