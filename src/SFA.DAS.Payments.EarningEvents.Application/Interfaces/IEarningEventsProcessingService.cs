using System.Collections.Generic;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using SFA.DAS.Payments.EarningEvents.Messages.Events;

namespace SFA.DAS.Payments.EarningEvents.Application.Interfaces
{
    public interface IEarningEventsProcessingService
    {
        IEnumerable<ApprenticeshipContractType2EarningEvent> GetEarningEvents(FM36Global learners);
    }
}