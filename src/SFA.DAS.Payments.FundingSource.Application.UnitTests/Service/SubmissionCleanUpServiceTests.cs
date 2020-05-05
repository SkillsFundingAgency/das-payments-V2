using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Payments.Application.Infrastructure.Logging;
using SFA.DAS.Payments.FundingSource.Application.Data;
using SFA.DAS.Payments.FundingSource.Application.Interfaces;
using SFA.DAS.Payments.FundingSource.Application.Services;
using SFA.DAS.Payments.Messages.Core.Commands;

namespace SFA.DAS.Payments.FundingSource.Application.UnitTests.Service
{
    [TestFixture]
    public class SubmissionCleanUpServiceTests
    {
        private ISubmissionCleanUpService submissionCleanUpService;
        private Mock<IFundingSourceDataContext> fundingSourceDataContext;


        [SetUp]
        public void SetUp()
        {
            fundingSourceDataContext =new  Mock<IFundingSourceDataContext>(); 
            var logger =new  Mock<IPaymentLogger>();

            submissionCleanUpService = new SubmissionCleanUpService(logger.Object, fundingSourceDataContext.Object);
        }


        [Test]
        public async Task RemovePreviousSubmissions_CallsDeletePreviousSubmissionsOnDbContextUsingCorrectParameters()
        {
            var commandJobId = 10001;
            var collectionPeriod = (byte)1;
            var academicYear = (short)1920;
            var commandSubmissionDate = DateTime.Now;
            var ukprn = 10000001l;
           

          await  submissionCleanUpService.RemovePreviousSubmissions(commandJobId, collectionPeriod, academicYear, commandSubmissionDate, ukprn);

           fundingSourceDataContext.Verify(x=> x.DeletePreviousSubmissions(commandJobId, collectionPeriod, academicYear, commandSubmissionDate, ukprn));
        }

        [Test]
        public async Task RemoveCurrentSubmission_CallsDeleteCurrentSubmissionsOnDbContextUsingCorrectParameters()
        {
            var commandJobId = 10001;
            var collectionPeriod = (byte)1;
            var academicYear = (short)1920;
            var ukprn = 10000001l;

            await  submissionCleanUpService.RemoveCurrentSubmission(commandJobId, collectionPeriod, academicYear, ukprn);

            fundingSourceDataContext.Verify(x=> x.DeleteCurrentSubmissions(commandJobId, collectionPeriod, academicYear, ukprn));
        }

    }
}