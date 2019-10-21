using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services.Apprenticeships
{
    public interface IApprenticeshipService
    {
        Task<List<ApprenticeshipDuplicateModel>> NewApprenticeship(ApprenticeshipModel apprenticeship);
        Task<List<ApprenticeshipModel>> GetUpdatedApprenticeshipEmployerIsLevyPayerFlag(long accountId, CancellationToken cancellation = default(CancellationToken));
    }

    public class ApprenticeshipService : IApprenticeshipService
    {
        private readonly IApprenticeshipRepository repository;
      

        public ApprenticeshipService(IApprenticeshipRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<List<ApprenticeshipDuplicateModel>> NewApprenticeship(ApprenticeshipModel newApprenticeship)
        {
            var apprenticeship = await repository.Get(newApprenticeship.Id);
            if (apprenticeship != null)
            {
                throw new InvalidOperationException($"Cannot store new apprenticeship as it already exists. Apprenticeship id: {newApprenticeship.Id}, employer: {newApprenticeship.AccountId}, ukprn: {newApprenticeship.Ukprn}");
            }

            using (var scope = new TransactionScope(TransactionScopeOption.Required, TransactionScopeAsyncFlowOption.Enabled))
            {
                await repository.Add(newApprenticeship);
                var duplicates = await repository.GetDuplicates(newApprenticeship.Uln);
                var providers = duplicates
                    .Select(duplicate => duplicate.Ukprn)
                    .Distinct()
                    .ToList();

                var newDuplicates = providers
                    .Select(ukprn => new ApprenticeshipDuplicateModel
                    {
                        ApprenticeshipId = newApprenticeship.Id,
                        Ukprn = ukprn,
                        Uln = newApprenticeship.Uln
                    })
                    .ToList();

                if (!providers.Contains(newApprenticeship.Ukprn))
                {
                    newDuplicates.Add(new ApprenticeshipDuplicateModel { Ukprn = newApprenticeship.Ukprn, ApprenticeshipId = newApprenticeship.Id, Uln = newApprenticeship.Uln });
                    newDuplicates.AddRange(duplicates.Select(duplicate => new ApprenticeshipDuplicateModel
                    {
                        ApprenticeshipId = duplicate.ApprenticeshipId,
                        Uln = newApprenticeship.Uln,
                        Ukprn = newApprenticeship.Ukprn
                    }));
                }

                await repository.StoreDuplicates(newDuplicates);
                scope.Complete();
                return duplicates;
            }
        }

        public async Task<List<ApprenticeshipModel>> GetUpdatedApprenticeshipEmployerIsLevyPayerFlag(long accountId,CancellationToken cancellation = default(CancellationToken))
        {
            var apprenticeships = await repository.GetEmployerApprenticeships(accountId, cancellation).ConfigureAwait(false);

            if (apprenticeships == null || apprenticeships.Count == 0)
            {
                return  new List<ApprenticeshipModel>();
            }

            apprenticeships = apprenticeships.Where(apprenticeship => apprenticeship.IsLevyPayer).ToList();
            apprenticeships.ForEach(x => x.IsLevyPayer = false);
            await  repository.UpdateApprenticeships(apprenticeships).ConfigureAwait(false);

            return apprenticeships;
        }


    }
}
