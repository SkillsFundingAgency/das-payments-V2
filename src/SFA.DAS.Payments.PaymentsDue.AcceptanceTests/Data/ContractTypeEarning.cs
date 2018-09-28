using System;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Data
{
    public class ContractTypeEarning
    {
        public string PriceEpisodeIdentifier { get; set; }

        public byte Delivery_Period { get; set; }

        public OnProgrammeEarningType TransactionType { get; set; }

        public decimal Amount { get; set; }
    }
}