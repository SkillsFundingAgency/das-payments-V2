using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public class Provider
    {
        public string Identifier { get; set; }
        public long Ukprn { get; set; }
        public long JobId { get; set; }
        public DateTime IlrSubmissionTime { get; set; }
    }
}
