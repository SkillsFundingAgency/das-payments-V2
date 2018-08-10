using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SFA.DAS.Payments.EarningEvents.Messages.Entities
{
    [DataContract]
    public class LearnAimEntity
    {
        [DataMember]
        public string LearnAimRef { get; set; }

        [DataMember]
        public int ProgrammeType { get; set; }

        [DataMember]
        public int StandardCode { get; set; }

        [DataMember]
        public int FrameworkCode { get; set; }

        [DataMember]
        public int PathwayCode { get; set; }

    }
}
