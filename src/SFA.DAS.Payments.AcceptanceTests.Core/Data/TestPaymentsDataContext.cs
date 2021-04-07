using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.AcceptanceTests.Core.Data.Configurations;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Monitoring.Jobs.Model;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Data
{
    public class TestPaymentsDataContext : PaymentsDataContext
    {
        public DbSet<TestModels.Provider> Providers { get; set; }

        public TestPaymentsDataContext(string connectionString) : base(connectionString)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString, options => options.CommandTimeout(600));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ProviderModelConfiguration());
        }

        public TestModels.Provider LeastRecentlyUsed() =>
            Providers.OrderBy(x => x.LastUsed).FirstOrDefault()
            ?? throw new InvalidOperationException("There are no UKPRNs available in the well-known Providers pool.");


        public async Task CreateSubmissionWindowValidationJob(short academicYear, byte collectionPeriod)
        {
            await CreateSubmissionWindowValidationJob(academicYear, collectionPeriod, JobStatus.Completed, new DateTimeOffset(DateTime.Now));
        }

        public async Task CreateSubmissionWindowValidationJob(short academicYear, byte collectionPeriod, JobStatus status)
        {
            await CreateSubmissionWindowValidationJob(academicYear, collectionPeriod, status, new DateTimeOffset(DateTime.Now));
        }

        public async Task CreateSubmissionWindowValidationJob(short academicYear, byte collectionPeriod, DateTimeOffset? endTime)
        {
            await CreateSubmissionWindowValidationJob(academicYear, collectionPeriod, JobStatus.Completed, endTime);
        }

        public async Task CreateSubmissionWindowValidationJob(short academicYear, byte collectionPeriod, JobStatus status, DateTimeOffset? endTime)
        {
            await DeleteSubmissionWindowValidationJob(academicYear, collectionPeriod).ConfigureAwait(false);
            const string sql = 
                @"INSERT INTO [Payments2].[Job] ([JobType] ,[StartTime] ,[EndTime] ,[Status] ,[CreationDate] ,[DCJobId] ,[Ukprn] ,[IlrSubmissionTime] ,[LearnerCount] ,[AcademicYear] ,[CollectionPeriod] ,[DataLocksCompletionTime] ,[DCJobSucceeded] ,[DCJobEndTime])
                  VALUES (    7, GETDATE(), {3}, {2}, GETDATE(), 1, null, GETDATE(), null, {0}, {1}, GETDATE(), 1, GETDATE())";

            await Database.ExecuteSqlCommandAsync(sql, academicYear, collectionPeriod, status, endTime).ConfigureAwait(false);
        }

        public async Task DeleteSubmissionWindowValidationJob(short academicYear, byte collectionPeriod)
        {
            const string sql = @"DELETE FROM [Payments2].[Job] WHERE AcademicYear = {0} AND CollectionPeriod = {1};";

            await Database.ExecuteSqlCommandAsync(sql, academicYear, collectionPeriod).ConfigureAwait(false);
        }

        public async Task DeleteCollectionPeriod(short academicYear, byte collectionPeriod)
        {
            const string sql = @"DELETE FROM [Payments2].[CollectionPeriod] WHERE AcademicYear = {0} AND Period = {1}";

            await Database.ExecuteSqlCommandAsync(sql, academicYear, collectionPeriod).ConfigureAwait(false);
        }

        public void ClearPaymentsData(long ukprn)
        {
            Database.ExecuteSqlCommand(DeleteUkprnData, ukprn);
        }

        public Task<int> ClearPaymentsDataAsync(long ukprn)
        {
            return Database.ExecuteSqlCommandAsync(DeleteUkprnData, ukprn);
        }

        private const string DeleteUkprnData = @"
            delete from Payments2.LevyAccount where AccountId in
	            (select AccountId from Payments2.Apprenticeship where Ukprn = {0})

            delete from Payments2.ApprenticeshipPriceEpisode where ApprenticeshipId in 
	            (select Id from Payments2.Apprenticeship where Ukprn = {0})

            delete from Payments2.ApprenticeshipPause where ApprenticeshipId in 
	            (select Id from Payments2.Apprenticeship where Ukprn = {0})

            delete from Payments2.ApprenticeshipDuplicate where ApprenticeshipId in
	            (select Id from Payments2.Apprenticeship where Ukprn = {0} )

            delete from Payments2.DataLockEventNonPayablePeriodFailures where ApprenticeshipId in
	            (select Id from Payments2.Apprenticeship where Ukprn = {0} )

            delete from Payments2.Apprenticeship where Ukprn = {0}

            delete from Payments2.DataLockEventNonPayablePeriod where DataLockEventId in 
	            (select EventId from Payments2.DataLockEvent where Ukprn = {0})

            delete from Payments2.DataLockEventPayablePeriod where DataLockEventId in 
	            (select EventId from Payments2.DataLockEvent where Ukprn = {0})

            delete from Payments2.DataLockEventPriceEpisode where DataLockEventId in 
	            (select EventId from Payments2.DataLockEvent where Ukprn = {0})

            delete from Payments2.DataLockFailure where Ukprn = {0}

            delete from Payments2.DataLockEvent where Ukprn = {0}

            delete from Payments2.EarningEventPeriod where EarningEventId in 
	            (select EventId from Payments2.EarningEvent where Ukprn = {0})

            delete from Payments2.EarningEventPriceEpisode where EarningEventId in 
	            (select EventId from Payments2.EarningEvent where Ukprn = {0})

            delete from Payments2.EarningEvent where Ukprn = {0}

            delete from Payments2.EmployerProviderPriority where Ukprn = {0}

            delete from Payments2.FundingSourceEvent where Ukprn = {0}

            delete from Payments2.RequiredPaymentEvent where Ukprn = {0}

            delete from Payments2.Payment where Ukprn = {0}

            delete from Payments2.SubmittedLearnerAim where Ukprn = {0}

            delete from Payments2.FundingSourceLevyTransaction where Ukprn = {0}
        ";

        public async Task ClearApprenticeshipData(long apprenticeshipId, long uln)
        {
            const string deleteApprenticeshipData = @"
                delete from Payments2.[ApprenticeshipDuplicate] where ApprenticeshipId in (select Id from Payments2.Apprenticeship where Id = {0} or Uln = {1})
                delete from Payments2.[ApprenticeshipPause] where ApprenticeshipId in (select Id from Payments2.Apprenticeship where Id = {0} or Uln = {1})
                delete from Payments2.[ApprenticeshipPriceEpisode] where ApprenticeshipId in (select Id from Payments2.Apprenticeship where Id = {0} or Uln = {1})
                delete from Payments2.[Apprenticeship] where Id = {0} or Uln = {1}
            ";

            await Database.ExecuteSqlCommandAsync(deleteApprenticeshipData, apprenticeshipId, uln).ConfigureAwait(false);
        }

        public void ClearJobFromDcJobId(long dcJobId)
        {
            Database.ExecuteSqlCommand($@"
                delete e from Payments2.JobEvent e join Payments2.Job j on j.JobId = e.JobId where j.DCJobId = {dcJobId}
                delete from Payments2.Job where DCJobId = {dcJobId}
            ");
        }

        public List<long> GetMatchingJobs(short jobType, byte collectionPeriod, short academicYear, byte status)
        {
            var matchingJobs = new List<long>();
            using (var connection = (SqlConnection)Database.GetDbConnection())
            {
                connection.Open();
                var reader = new SqlCommand($@"
                    SELECT DcJobId
                    FROM Payments2.Job
                    WHERE JobType = {jobType}
                    AND CollectionPeriod = {collectionPeriod}
                    AND AcademicYear = {academicYear}
                    AND[Status] = {status}", connection).ExecuteReader();

                while (reader.Read()) { matchingJobs.Add(long.Parse(reader["DcJobId"].ToString())); }
            }
            return matchingJobs;
        }

        public bool JobCompleted(long jobId, short jobType)
        {
            using (var connection = (SqlConnection)Database.GetDbConnection())
            {
                connection.Open();
                return new SqlCommand($"SELECT JobId FROM Payments2.Job WHERE DCJobId = {jobId} AND JobType = {jobType} AND [Status] = 2", connection).ExecuteReader().HasRows;
            }
        }

        public void SetJobToCompletedWithErrors(long dcJobId)
        {
            Database.ExecuteSqlCommand($@"
                UPDATE Payments2.Job SET [Status] = 3 WHERE DCJobId = {dcJobId}
            ");
        }
    }
}