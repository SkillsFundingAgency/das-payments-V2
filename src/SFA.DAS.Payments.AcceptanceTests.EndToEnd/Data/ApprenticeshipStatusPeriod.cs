using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data
{
    public class ApprenticeshipStatusPeriod
    {
        public string Identifier { get; set; } = "Apprenticeship 1";
        public string CollectionPeriod { get; set; }
        public ApprenticeshipStatus Status { get; set; } 
        public string StoppedDate { get; set; }
    }
}