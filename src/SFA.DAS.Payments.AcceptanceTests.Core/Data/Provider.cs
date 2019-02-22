using System;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public class Provider
    {
        public string Identifier { get; set; }
        public long Ukprn { get; set; }
        public long JobId { get; set; }
        public DateTime IlrSubmissionTime { get; set; }
        public bool MonthEndJobIdGenerated { get; set; }
    }
}
