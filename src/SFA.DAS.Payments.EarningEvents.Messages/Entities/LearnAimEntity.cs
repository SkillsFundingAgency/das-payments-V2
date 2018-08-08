using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.EarningEvents.Messages.Entities
{
    public class LearnAimEntity
    {
        public string LearnAimRef { get; set; }

        public int ProgrammeType { get; set; }

        public int StandardCode { get; set; }

        public int FrameworkCode { get; set; }

        public int PathwayCode { get; set; }

    }
}
