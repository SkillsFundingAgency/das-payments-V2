using System;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.ProviderPayments.Application.Data;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public class CollectionPeriodStorageService : ICollectionPeriodStorageService
    {
        private readonly IProviderPaymentsDataContext context;
        private readonly IPaymentLogger logger;

        public CollectionPeriodStorageService(IProviderPaymentsDataContext context, IPaymentLogger logger)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task StoreCollectionPeriod(short academicYear, byte period, DateTime completionDateTime)
        {
            if(context.CollectionPeriod.Any(x => x.AcademicYear == academicYear && x.Period == period))
                return;

            var referenceDataValidationDate = GetReferenceDataValidationDate(academicYear, period);
            if(referenceDataValidationDate == null)
                logger.LogWarning($"Failed to find successful PeriodEndSubmissionWindowValidationJob for academic year: {academicYear} and period: {period} with an EndTime set");

            context.CollectionPeriod.Add(new CollectionPeriodModel
            {
                AcademicYear = academicYear,
                Period = period,
                CalendarMonth = GetCalendarMonth(period),
                CalendarYear = GetCalendarYear(academicYear, period),
                CompletionDate = completionDateTime,
                ReferenceDataValidationDate = referenceDataValidationDate
            });
            await context.SaveChanges();
        }

        private byte GetCalendarMonth(byte period)
        {
            if (period < 6)
                return (byte)(period + 7);
            if (period == 13)
                return 9;
            if (period == 14)
                return 10;
            return (byte)(period - 5);
        }

        private short GetCalendarYear(short academicYear, byte period)
        {
            if (period < 6)
                return short.Parse($"20{academicYear.ToString().Substring(0, 2)}");
            return short.Parse($"20{academicYear.ToString().Substring(2, 2)}");
        }

        private DateTime? GetReferenceDataValidationDate(short academicYear, byte period)
        {
            return context.Job.Where(x => x.JobType == JobType.PeriodEndSubmissionWindowValidationJob
                                    && x.AcademicYear == academicYear
                                    && x.CollectionPeriod == period
                                    && x.EndTime != null
                                    && x.Status == JobStatus.Completed)
                .OrderByDescending(x => x.EndTime)
                .FirstOrDefault()?.EndTime?.DateTime;
        }
    }
}