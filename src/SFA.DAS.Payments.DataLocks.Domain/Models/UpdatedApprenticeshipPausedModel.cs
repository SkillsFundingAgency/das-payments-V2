using System;


namespace SFA.DAS.Payments.DataLocks.Domain.Models
{
    public class UpdatedApprenticeshipPausedModel: UpdatedApprenticeshipModel
    {
        public DateTime PauseDate { get; set; }

    }
}
