using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.Monitoring.Metrics.Model.PeriodEnd
{
    public class CollectionPeriodToleranceModel
    {
		public int Id { get; set; }
		public byte CollectionPeriod { get; set; }
		public short AcademicYear { get; set; }
		public decimal SubmissionToleranceLower { get; set; }
		public decimal SubmissionToleranceUpper { get; set; }
		public decimal PeriodEndToleranceLower { get; set; }
		public decimal PeriodEndToleranceUpper { get; set; }
	}
}
