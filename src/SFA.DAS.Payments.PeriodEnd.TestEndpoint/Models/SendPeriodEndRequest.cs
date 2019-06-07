using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.PeriodEnd.TestEndpoint.Models
{
    public class SendPeriodEndRequest
    {
        public long Ukprn { get; set; }
        public short AcademicYear { get; set; }
        public byte Period { get; set; }
    }
}
