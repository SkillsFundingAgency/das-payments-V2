using System;
using System.Collections.Generic;
using SFA.DAS.Payments.Messages.Core.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Messages.Events
{
    public class ApprenticeshipUpdated: IEvent
    {
        
        public long Id { get; set; }
        public long EmployerAccountId { get; set; }
        public string AgreementId { get; set; }
        public DateTime AgreedOnDate { get; set; }
        public long Uln { get; set; }
        public long Ukprn { get; set; }
        public DateTime EstimatedStartDate { get; set; }
        public DateTime EstimatedEndDate { get; set; }
        public long? StandardCode { get; set; }
        public int? ProgrammeType { get; set; }
        public int? FrameworkCode { get; set; }
        public int? PathwayCode { get; set; }
        public string LegalEntityName { get; set; }
        public long? TransferSendingEmployerAccountId { get; set; }
        public DateTime? StopDate { get; set; }
        public ApprenticeshipStatus Status { get; set; }
        public bool IsLevyPayer { get; set; }
        public List<ApprenticeshipPriceEpisodeModel> ApprenticeshipPriceEpisodes { get; set; }
        public List<ApprenticeshipDuplicate> Duplicates { get; set; }
        public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }
        public Guid EventId { get; set; }
        public DateTimeOffset EventTime { get; set; }

        public ApprenticeshipUpdated()
        {
            EventId = Guid.NewGuid();
            EventTime = DateTimeOffset.UtcNow;
            ApprenticeshipPriceEpisodes = new List<ApprenticeshipPriceEpisodeModel>();
            Duplicates = new List<ApprenticeshipDuplicate>();
        }

    }
}