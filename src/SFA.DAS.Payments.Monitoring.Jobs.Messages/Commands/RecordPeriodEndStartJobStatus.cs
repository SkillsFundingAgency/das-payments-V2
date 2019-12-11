using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Payments.Monitoring.Jobs.Messages.Commands
{
  public abstract class RecordPeriodEndStartJobStatus: JobsCommand
    {
        public byte CollectionPeriod { get; set; }
        public short AcademicYear { get; set; }
    }
}
