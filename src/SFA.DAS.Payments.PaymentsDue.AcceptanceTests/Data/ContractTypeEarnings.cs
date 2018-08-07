using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Data
{
    public class ContractTypeEarnings
    {
        public ContractTypeEarnings(short contractType, short fromPeriod, short toPeriod, IReadOnlyCollection<OnProgrammeEarning> rawEarnings)
        {
            ContractType = contractType;

            FromPeriod = fromPeriod;

            ToPeriod = toPeriod;

            OnProgrammeEarnings = rawEarnings.ToList();
        }

        public short ToPeriod { get;  }

        public short FromPeriod { get;  }

        public short ContractType { get; } 

        public List<OnProgrammeEarning> OnProgrammeEarnings { get; } 
    }
}