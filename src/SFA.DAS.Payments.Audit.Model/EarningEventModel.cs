using System;
using System.Collections.Generic;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.Audit.Model
{
    public class EarningEventModel: PaymentsEventModel
    {
        public ContractType ContractType { get; set; }
        public string AgreementId { get; set; }
        public List<EarningEventPeriodModel> Periods { get; set; }
        public List<EarningEventPriceEpisodeModel> PriceEpisodes { get; set; }
    }

    public class EarningEventPeriodModel
    {
        public Guid EarningEventId { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public TransactionType TransactionType { get; set; }
        public byte DeliveryPeriod { get; set; }
        public decimal Amount { get; set; }
    }

    public class EarningEventPriceEpisodeModel
    {
        public Guid EarningEventId { get; set; }
        public string PriceEpisodeIdentifier { get; set; }
        public decimal SfaContributionPercentage { get; set; }
        public decimal TotalNegotiatedPrice1 { get; set; }
        public decimal TotalNegotiatedPrice2 { get; set; }
        public decimal TotalNegotiatedPrice3 { get; set; }
        public decimal TotalNegotiatedPrice4 { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime PlannedEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public int NumberOfInstalments { get; set; }
        public decimal InstalmentAmount { get; set; }
        public decimal CompletionAmount { get; set; }
        public bool Completed { get; set; }
    }
}