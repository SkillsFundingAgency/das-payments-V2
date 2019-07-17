using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Audit.Model
{
    public class EarningEventModel: PaymentsEventModel
    {
        public ContractType ContractType { get; set; }
        public string AgreementId { get; set; }
        public int? LearningAimSequenceNumber { get; set; }
        public List<EarningEventPeriodModel> Periods { get; set; }
        public List<EarningEventPriceEpisodeModel> PriceEpisodes { get; set; }
    }
}
