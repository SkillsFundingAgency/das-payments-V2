﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships
{
    public interface IApprenticeshipRepository
    {
        Task<List<ApprenticeshipModel>> ApprenticeshipsForProvider(long ukprn);
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
    }
}