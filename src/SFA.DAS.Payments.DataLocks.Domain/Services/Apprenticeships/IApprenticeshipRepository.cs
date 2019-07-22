using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships
{
    public interface IApprenticeshipRepository: IDisposable
    {
        Task<List<long>> GetProviderIds();
        Task<List<long>> ApprenticeshipUlnsByProvider(long ukprn);
        Task<List<ApprenticeshipModel>> ApprenticeshipsByUln(long uln);
        Task<List<ApprenticeshipModel>> DuplicateApprenticeshipsForProvider(long ukprn);
        Task<ApprenticeshipModel> Get(long apprenticeshipId);
        Task<List<ApprenticeshipDuplicateModel>> GetDuplicates(long uln);
        Task Add(ApprenticeshipModel apprenticeship);
        Task StoreDuplicates(List<ApprenticeshipDuplicateModel> duplicates);
    }
}