using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.DataLocks.Model.Entities;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Application.Repositories
{
    public interface IDataLockFailureRepository
    {
        Task<List<DataLockFailureEntity>> GetFailures(
            long ukprn,
            string learnerReferenceNumber,
            int frameworkCode,
            int pathwayCode,
            int programmeType,
            int standardCode,
            string learnAimRef,
            short academicYear
        );

        Task ReplaceFailures(List<long> oldFailureIds, List<DataLockFailureEntity> newFailures);
    }

    public class DataLockFailureRepository : IDataLockFailureRepository
    {
        private readonly IPaymentsDataContext paymentsDataContext;
        private IPaymentLogger logger;

        public DataLockFailureRepository(IPaymentsDataContext paymentsDataContext, IPaymentLogger logger)
        {
            this.paymentsDataContext = paymentsDataContext;
            this.logger = logger;
        }

        public async Task<List<DataLockFailureEntity>> GetFailures(long ukprn, string learnerReferenceNumber, int frameworkCode, int pathwayCode, int programmeType, int standardCode, string learnAimRef, short academicYear)
        {
            var entities = await paymentsDataContext.DataLockFailure.Where(f =>
                    f.Ukprn == ukprn &&
                    f.LearnerReferenceNumber == learnerReferenceNumber &&
                    f.LearningAimFrameworkCode == frameworkCode &&
                    f.LearningAimPathwayCode == pathwayCode &&
                    f.LearningAimProgrammeType == programmeType &&
                    f.LearningAimStandardCode == standardCode &&
                    f.LearningAimReference == learnAimRef &&
                    f.AcademicYear == academicYear
                )
                .Select(model => new DataLockFailureEntity
                {
                    Ukprn = model.Ukprn,
                    AcademicYear = model.AcademicYear,
                    TransactionType = model.TransactionType,
                    DeliveryPeriod = model.DeliveryPeriod,
                    Id = model.Id,
                    LearnerReferenceNumber = model.LearnerReferenceNumber,
                    LearnerUln = model.LearnerUln,
                    LearningAimFrameworkCode = model.LearningAimFrameworkCode,
                    LearningAimPathwayCode = model.LearningAimPathwayCode,
                    LearningAimProgrammeType = model.LearningAimProgrammeType,
                    LearningAimReference = model.LearningAimReference,
                    LearningAimStandardCode = model.LearningAimStandardCode,
                    EarningPeriod = JsonConvert.DeserializeObject<EarningPeriod>(model.EarningPeriod)
                })
                .ToListAsync()
                .ConfigureAwait(false);

            logger.LogDebug($"retrieved {entities.Count} errors for UKPRN {ukprn}");

            return entities;
        }

        public async Task ReplaceFailures(List<long> oldFailureIds, List<DataLockFailureEntity> newFailures)
        {
            if (oldFailureIds.Count > 0)
                logger.LogDebug($"deleting {oldFailureIds.Count} errors");
            if (newFailures.Count > 0)
                logger.LogDebug($"adding {newFailures.Count} new failures for UKPRN {newFailures[0].Ukprn}");

            paymentsDataContext.DataLockFailure.RemoveRange(paymentsDataContext.DataLockFailure.Where(f => oldFailureIds.Contains(f.Id)));

            await paymentsDataContext.DataLockFailure.AddRangeAsync(newFailures.Select(f => new DataLockFailureModel
            {
                CollectionPeriod = f.CollectionPeriod,
                AcademicYear = f.AcademicYear,
                Ukprn = f.Ukprn,
                TransactionType = f.TransactionType,
                DeliveryPeriod = f.DeliveryPeriod,
                LearnerReferenceNumber = f.LearnerReferenceNumber,
                LearnerUln = f.LearnerUln,
                LearningAimFrameworkCode = f.LearningAimFrameworkCode,
                LearningAimPathwayCode = f.LearningAimPathwayCode,
                LearningAimProgrammeType = f.LearningAimProgrammeType,
                LearningAimReference = f.LearningAimReference,
                LearningAimStandardCode = f.LearningAimStandardCode,
                EarningPeriod = JsonConvert.SerializeObject(f.EarningPeriod)
            })).ConfigureAwait(false);

            await paymentsDataContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
