using System;
using System.Threading.Tasks;

namespace SFA.DAS.Payments.FundingSource.Application.Interfaces
{
    public interface ISubmissionCleanUpService
    {
        Task RemovePreviousSubmissions(long commandJobId, byte collectionPeriod, short academicYear, DateTime commandSubmissionDate, long ukprn);
        Task RemoveCurrentSubmission(long commandJobId, byte collectionPeriod, short academicYear, long ukprn);
    }
}