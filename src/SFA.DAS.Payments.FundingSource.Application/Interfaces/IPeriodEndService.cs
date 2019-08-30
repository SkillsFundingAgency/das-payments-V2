using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.FundingSource.Messages.Internal.Commands;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface IPeriodEndService
    {
        Task<List<ProcessLevyPaymentsOnMonthEndCommand>> GenerateEmployerPeriodEndCommands(
            PeriodEndRunningEvent message);
    }
}