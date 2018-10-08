using System.Collections.Generic;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Application.Interfaces
{
    public interface IEarningEventMapper
    {
        IEnumerable<ApprenticeshipContractType2EarningEvent> MapEarningEvent(FM36Global fm36Output);
    }
}