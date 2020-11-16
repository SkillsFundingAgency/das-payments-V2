using System;
using Microsoft.EntityFrameworkCore;

namespace SFA.DAS.Payments.Monitoring.Metrics.Function.AcceptanceTests.Data
{
    public class SubmissionJobsTestDataBuilder
    {
        private readonly TestSubmissionJobsDataContext dataContext;

        public long TestUkprn { get; }
        public long TestDcJobId => 987654321;

        public short AcademicYear { get; }
        public byte CollectionPeriod { get; }

        public SubmissionJobsTestDataBuilder(TestSubmissionJobsDataContext dataContext, short academicYear, byte collectionPeriod)
        {
            this.dataContext = dataContext;
            AcademicYear = academicYear;
            CollectionPeriod = collectionPeriod;
            TestUkprn = 123456789 + random.Next();
        }

        private JobsModel currentModel = null;
        private readonly Random random = new Random();

        private JobsModel GetCurrentJob()
        {
            if (currentModel == null)
                currentModel = new JobsModel
                {
                    AcademicYear = AcademicYear, 
                    CollectionPeriod = CollectionPeriod,
                    JobType = 1,
                    Ukprn = TestUkprn,
                    DcJobId = TestDcJobId,
                    Status = 2,
                    IlrSubmissionTime = DateTime.Now.AddMinutes(random.Next(2000)),
                };

            return currentModel;
        }

        public SubmissionJobsTestDataBuilder WithSuccessfulDcJob()
        {
            GetCurrentJob().DcJobSucceeded = true;
            return this;
        }

        public SubmissionJobsTestDataBuilder WithFailedDcJob()
        {
            GetCurrentJob().DcJobSucceeded = false;
            return this;
        }

        public SubmissionJobsTestDataBuilder WithSuccessfulDasJob()
        {
            GetCurrentJob().Status = (byte)random.Next(2, 3);
            return this;
        }

        public SubmissionJobsTestDataBuilder WithFailedDasJob()
        {
            GetCurrentJob().Status = 5;
            return this;
        }

        public void AddToDatabase()
        {
            if (currentModel != null)
            {
                dataContext.Jobs.Add(GetCurrentJob());
                dataContext.SaveChanges();
                currentModel = null;
            }
        }

        public void ClearDatabase()
        {
            dataContext.Database.ExecuteSqlCommand($"DELETE Payments2.Job WHERE AcademicYear = {AcademicYear} " +
                                                   $"AND CollectionPeriod = {CollectionPeriod} " +
                                                   $"AND Ukprn = {TestUkprn}");
        }
    }
}
