using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.PaymentsDue.AcceptanceTests.Data
{
    public class ContractTypeEarnings
    {
        public ContractTypeEarnings(short contractType, short fromPeriod, short toPeriod)
        {
            ContractType = contractType;

            FromPeriod = fromPeriod;

            ToPeriod = toPeriod;
        }

        public ContractTypeEarnings(short contractType, short fromPeriod, short toPeriod, IEnumerable<OnProgrammeEarning> rawEarnings)
        :this(contractType, fromPeriod, toPeriod)
        {
            OnProgrammeEarnings = rawEarnings.ToList();
        }

        public ContractTypeEarnings(short contractType, short fromPeriod, short toPeriod, IEnumerable<IncentiveEarning> rawEarnings)
            : this(contractType, fromPeriod, toPeriod)
        {
            IncentiveEarnings = rawEarnings.ToList();
        }

        public short ToPeriod { get;  }

        public short FromPeriod { get;  }

        public short ContractType { get; } 

        public List<OnProgrammeEarning> OnProgrammeEarnings { get; } 

        public List<IncentiveEarning> IncentiveEarnings { get; }
    }
}