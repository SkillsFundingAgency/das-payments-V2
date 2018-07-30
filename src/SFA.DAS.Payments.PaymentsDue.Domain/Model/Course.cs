using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.Payments.PaymentsDue.Domain.Enum;

namespace SFA.DAS.Payments.PaymentsDue.Domain.Model
{
    public class Course
    {
        public ProgrammeType ProgrammeType { get; set; }

        public int StandardCode { get; set; }

        public int FrameworkCode { get; set; }

        public int PathwayCode { get; set; }
    }
}
