
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.RequiredPayments.Domain.Services
{
    public class ApprenticeshipKey
    {
        public long Ukprn { get; set; }
        public string LearnerReferenceNumber { get; set; }
        public int FrameworkCode { get; set; }
        public int PathwayCode { get; set; }
        public int ProgrammeType { get; set; }
        public int StandardCode { get; set; }
        public string LearnAimRef { get; set; }
        public short AcademicYear { get; set; }
        public ContractType ContractType { get; set; }
    }
}