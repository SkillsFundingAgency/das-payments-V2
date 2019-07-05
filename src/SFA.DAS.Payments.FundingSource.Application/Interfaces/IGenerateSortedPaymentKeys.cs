using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface IGenerateSortedPaymentKeys
    {
        Task<List<string>> GeyKeys();
    }
}