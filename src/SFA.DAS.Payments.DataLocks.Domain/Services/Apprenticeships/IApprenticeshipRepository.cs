using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Audit;
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
        Task<List<ApprenticeshipModel>> Get(List<long> apprenticeshipIds, CancellationToken cancellationToken);
        Task<List<ApprenticeshipDuplicateModel>> GetDuplicates(long uln);
        Task Add(ApprenticeshipModel apprenticeship);
        Task StoreDuplicates(List<ApprenticeshipDuplicateModel> duplicates);
        Task UpdateApprenticeship(ApprenticeshipModel updatedApprenticeship);
        Task AddApprenticeshipPause(ApprenticeshipPauseModel pauseModel);
        Task<ApprenticeshipPauseModel> GetCurrentApprenticeshipPausedModel(long apprenticeshipId);
        Task UpdateCurrentlyPausedApprenticeship(ApprenticeshipPauseModel apprenticeshipPauseModel);
        Task<List<ApprenticeshipModel>> GetEmployerApprenticeships(long accountId, CancellationToken cancellationToken);
        Task UpdateApprenticeships(List<ApprenticeshipModel> updatedApprenticeships);
        Task<List<EarningEventModel>> GetProviderApprenticeshipEarnings(long uln, long ukprn, CancellationToken cancellationToken);
    }
}