using System.Collections.Generic;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public class LearnerEarningsHistory
    {
        public AdditionalIlrData AdditionalData { get; set; }

        public IEnumerable<Earning> PreviousEarnings { get; set; }
    }
}
