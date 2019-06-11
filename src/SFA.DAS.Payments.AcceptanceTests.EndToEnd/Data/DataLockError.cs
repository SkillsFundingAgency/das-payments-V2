using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    public class DataLockError
    {
        public string Apprenticeship { get; set; }
        public string LearnerId { get; set; }
        public string IlrStartDate { get; set; }
        public string DeliveryPeriod { get; set; }
        public TransactionType TransactionType { get; set; }
        public DataLockErrorCode ErrorCode { get; set; }
        public  int StandardCode { get; set; }
        public int ProgrammeType { get; set; }
        public int FrameworkCode { get; set; }
        public int PathwayCode { get; set; }
        public long? ApprenticeshipId { get; set; }

        public string PriceEpisodeIdentifier { get; set; }

    }
}
