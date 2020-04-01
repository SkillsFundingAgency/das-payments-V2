using System;

namespace SFA.DAS.Payments.DataLocks.Domain.Models
{
    public class UpdatedApprenticeshipApprovedModel: UpdatedApprenticeshipModel
    {
        public long Uln { get; set; }
        public DateTime EstimatedStartDate { get; set; }
        public DateTime EstimatedEndDate { get; set; }
    }
}
