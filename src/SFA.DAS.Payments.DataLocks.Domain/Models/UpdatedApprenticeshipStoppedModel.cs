using System;

namespace SFA.DAS.Payments.DataLocks.Domain.Models
{
    public class UpdatedApprenticeshipStoppedModel: BaseUpdatedApprenticeshipModel
    {
        public DateTime? StopDate { get; set; }

    }
}
