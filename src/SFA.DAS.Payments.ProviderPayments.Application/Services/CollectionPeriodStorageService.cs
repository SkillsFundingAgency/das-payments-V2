using System;
using System.Linq;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Monitoring.Jobs.Model;
using SFA.DAS.Payments.ProviderPayments.Application.Data;

namespace SFA.DAS.Payments.ProviderPayments.Application.Services
{
    public class CollectionPeriodStorageService : ICollectionPeriodService
    {
        private IProviderPaymentsDataContext _context;

        public CollectionPeriodStorageService(IProviderPaymentsDataContext context)
        {
            _context = context;
        }

        public void StoreCollectionPeriod(short academicYear, byte period, DateTime completionDateTime)
        {
            if(_context.CollectionPeriod.Any(x => x.AcademicYear == academicYear && x.Period == period))
                return;

            _context.CollectionPeriod.Add(new CollectionPeriodModel
            {
                AcademicYear = academicYear,
                Period = period,
                CalendarMonth = GetCalendarMonth(period),
                CalendarYear = GetCalendarYear(academicYear, period),
                CompletionDate = completionDateTime,
                ReferenceDataValidationDate = GetReferenceDataValidationDate(academicYear, period).GetValueOrDefault()
            });
            _context.SaveChanges();
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
            return _context.Job.Where(x => x.JobType == JobType.PeriodEndSubmissionWindowValidationJob
                                    && x.AcademicYear == academicYear
                                    && x.CollectionPeriod == period
                                    && x.EndTime != null
                                    && x.Status == JobStatus.Completed)
                .OrderByDescending(x => x.EndTime)
                .FirstOrDefault()?.EndTime?.DateTime;
        }
    }
}