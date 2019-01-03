using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.Model.Core.Incentives
{
    public class IncentiveEarning : Earning
    {
        public IncentiveEarningType Type { get; set; }
    }
}