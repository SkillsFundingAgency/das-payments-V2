﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Application.Data;

namespace SFA.DAS.Payments.Application.Repositories
{
    public interface IPaymentsDataContext
    {
        DbSet<LevyAccountModel> LevyAccount { get; }
        DbSet<PaymentModel> Payment { get; }
        DbSet<ApprenticeshipModel> Apprenticeship { get; }
        DbSet<ApprenticeshipPriceEpisodeModel> ApprenticeshipPriceEpisode { get; }
        DbSet<SubmittedLearnerAimModel> SubmittedLearnerAim { get; }
        DbSet<ApprenticeshipDuplicateModel> ApprenticeshipDuplicate { get; }
        DbSet<DataLockFailureModel> DataLockFailure { get; }
        DbSet<EmployerProviderPriorityModel> EmployerProviderPriority { get; }
        DbSet<ApprenticeshipPauseModel> ApprenticeshipPause { get; }
        DbSet<EarningEventModel> EarningEvent { get; }
        DbSet<EarningEventPeriodModel> EarningEventPeriod { get; }
        DbSet<EarningEventPriceEpisodeModel> EarningEventPriceEpisode { get; }
        DbSet<PaymentModelWithRequiredPaymentId> PaymentsWithRequiredPayments { get; }
        DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
        int SaveChanges();
    }
}