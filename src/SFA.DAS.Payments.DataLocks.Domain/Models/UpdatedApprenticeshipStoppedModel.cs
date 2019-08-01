using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Models
{
    public class UpdatedApprenticeshipStoppedModel: BaseUpdatedApprenticeshipModel
    {
        public DateTime? StopDate { get; set; }

    }
}
