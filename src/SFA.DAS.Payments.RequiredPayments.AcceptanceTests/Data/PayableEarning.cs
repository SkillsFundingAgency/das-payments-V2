using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.RequiredPayments.AcceptanceTests.Data
{
    public class PayableEarning
    {
        public string LearnRefNumber { get; set; }

        public long Ukprn { get; set; }

        public string PriceEpisodeIdentifier { get; set; }

        public short Period { get; set; }

        public long Uln { get; set; }

        public string TransactionType { get; set; }

        public decimal Amount { get; set; }

        public OnProgrammeEarningType? OnProgrammeEarningType
        {
            get
            {
                switch (TransactionType)
                {
                    case "Learning_1":
                            return Model.Core.OnProgramme.OnProgrammeEarningType.Learning;
                    case "Completion_2":
                        return Model.Core.OnProgramme.OnProgrammeEarningType.Completion;
                    case "Balancing_3":
                        return Model.Core.OnProgramme.OnProgrammeEarningType.Balancing;
                    default:
                        return null;
                }
            }
        }
    }
}