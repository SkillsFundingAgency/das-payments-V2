using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Payments.Model.Core.Entities;

namespace PaymentTools.Model
{
    public class Commitment
    {
        public long Id { get; set; }

        public string Description => $"{Start:MMM-yy} - {End:MMM-yy}";
        public long Provider { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset? End { get; set; }

        public List<ICommitmentItem> Items => Payments.Cast<ICommitmentItem>().Union(DataLocked).ToList();
        public List<DataLock> DataLocked { get; set; } = new List<DataLock>();
        public List<Payment> Payments { get; set; } = new List<Payment>();

        public decimal TotalPayments => Items.Where(x => x is Payment).Sum(x => (x as Payment).Amount);

        public long Employer { get; internal set; }
        public decimal Cost { get; internal set; }
        public ApprenticeshipStatus Status { get; internal set; }
        public string Course { get; internal set; }
        public long? StandardCode { get; set; }
        public int? FrameworkCode { get; set; }
        public int? PathwayCode { get; set; }
        public int? ProgrammeType { get; set; }
        public long Ukprn { get; set; }
        public long Uln { get; set; }
    }
}