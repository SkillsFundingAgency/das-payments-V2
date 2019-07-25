using System;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Model.Entities;

namespace SFA.DAS.Payments.EarningEvents.Domain.UnitTests
{
    [TestFixture]
    public class SubmissionEventProcessorTest
    {
        private ISubmissionEventProcessor processor;

        [SetUp]
        public void Arrange()
        {
            processor = new SubmissionEventProcessor();
        }

        [Test]
        public void LeavesOriginalSubmissionIfItIsFirst()
        {
            // Arrange
            var ilrForFirstSubmission = new SubmittedPriceEpisodeEntity
            {
                Ukprn = 123456,
                LearnRefNumber = "1",
                PriceEpisodeIdentifier = "00-34-01/2016/12",
                IlrDetails = new IlrDetails
                {
                    IlrFileName = "ILR-123456-1617-20170101-000000-01.xml",
                    FileDateTime = new DateTime(2017, 1, 1),
                    SubmittedDateTime = new DateTime(2017, 1, 1, 12, 36, 23),
                    Uln = 987654,
                    AimSeqNumber = 1,
                    StandardCode = 34,
                    ActualEndDate = new DateTime(2016, 12, 13),
                    ActualStartDate = new DateTime(2014, 12, 13),
                    PlannedEndDate = new DateTime(2018, 2, 1),
                    OnProgrammeTotalPrice = 12000,
                    CompletionTotalPrice = 3000,
                    NiNumber = "AB123456A",
                    CommitmentId = 12345L,
                    EmployerReferenceNumber = 1005,
                    EPAOrgId = "EPA0001",
                    GivenNames = "James",
                    FamilyName = "Kirk",
                    CompStatus = 1
                }
            };

            // Act
            var event1 = processor.ProcessSubmission(ilrForFirstSubmission, null);

            // Assert
            Assert.AreEqual(event1.IlrFileName, ilrForFirstSubmission.IlrDetails.IlrFileName);
            Assert.AreEqual(event1.FileDateTime, ilrForFirstSubmission.IlrDetails.FileDateTime);
            Assert.AreEqual(event1.SubmittedDateTime, ilrForFirstSubmission.IlrDetails.SubmittedDateTime);
            Assert.AreEqual(event1.ComponentVersionNumber, 2);
            Assert.AreEqual(event1.UKPRN, ilrForFirstSubmission.Ukprn);
            Assert.AreEqual(event1.ULN, ilrForFirstSubmission.IlrDetails.Uln);
            Assert.AreEqual(event1.LearnRefNumber, ilrForFirstSubmission.LearnRefNumber);
            Assert.AreEqual(event1.AimSeqNumber, ilrForFirstSubmission.IlrDetails.AimSeqNumber);
            Assert.AreEqual(event1.PriceEpisodeIdentifier, ilrForFirstSubmission.PriceEpisodeIdentifier);
            Assert.AreEqual(event1.StandardCode, ilrForFirstSubmission.IlrDetails.StandardCode);
            Assert.AreEqual(event1.ProgrammeType, ilrForFirstSubmission.IlrDetails.ProgrammeType);
            Assert.AreEqual(event1.FrameworkCode, ilrForFirstSubmission.IlrDetails.FrameworkCode);
            Assert.AreEqual(event1.PathwayCode, ilrForFirstSubmission.IlrDetails.PathwayCode);
            Assert.AreEqual(event1.ActualStartDate, ilrForFirstSubmission.IlrDetails.ActualStartDate);
            Assert.AreEqual(event1.PlannedEndDate, ilrForFirstSubmission.IlrDetails.PlannedEndDate);
            Assert.AreEqual(event1.ActualEndDate, ilrForFirstSubmission.IlrDetails.ActualEndDate);
            Assert.AreEqual(event1.OnProgrammeTotalPrice, ilrForFirstSubmission.IlrDetails.OnProgrammeTotalPrice);
            Assert.AreEqual(event1.CompletionTotalPrice, ilrForFirstSubmission.IlrDetails.CompletionTotalPrice);
            Assert.AreEqual(event1.NINumber, ilrForFirstSubmission.IlrDetails.NiNumber);
            Assert.AreEqual(event1.CommitmentId, ilrForFirstSubmission.IlrDetails.CommitmentId);
            Assert.AreEqual(event1.EPAOrgId, ilrForFirstSubmission.IlrDetails.EPAOrgId);
            Assert.AreEqual(event1.GivenNames, ilrForFirstSubmission.IlrDetails.GivenNames);
            Assert.AreEqual(event1.FamilyName, ilrForFirstSubmission.IlrDetails.FamilyName);
            Assert.AreEqual(event1.CompStatus, ilrForFirstSubmission.IlrDetails.CompStatus);
            Assert.AreEqual(event1.EmployerReferenceNumber, ilrForFirstSubmission.IlrDetails.EmployerReferenceNumber);
        }


        [Test]
        public void ReturnsEventIfIlrHasChanged()
        {
            // Arrange
            var updatedSubmissionOriginal = new SubmittedPriceEpisodeEntity
            {
                Ukprn = 654789,
                LearnRefNumber = "1",
                PriceEpisodeIdentifier = "00-34-01/2016/12",
                IlrDetails = new IlrDetails
                {
                    IlrFileName = "ILR-123456-1617-20170101-000000-01.xml",
                    FileDateTime = new DateTime(2017, 1, 1),
                    SubmittedDateTime = new DateTime(2017, 1, 1, 12, 36, 23),
                    Uln = 987654,
                    AimSeqNumber = 1,
                    StandardCode = 34,
                    ActualEndDate = new DateTime(2016, 12, 13),
                    ActualStartDate = new DateTime(2014, 12, 13),
                    PlannedEndDate = new DateTime(2018, 2, 1),
                    OnProgrammeTotalPrice = 12000,
                    CompletionTotalPrice = 3000,
                    NiNumber = "AB123456A",
                    CommitmentId = 12345L,
                    EmployerReferenceNumber = 1005,
                    EPAOrgId = "EPA0001",
                    GivenNames = "James",
                    FamilyName = "Kirk",
                    CompStatus = 1
                }
            };

            var updatedSubmissionChanged = new SubmittedPriceEpisodeEntity
            {
                Ukprn = 654789,
                LearnRefNumber = "1",
                PriceEpisodeIdentifier = "00-34-01/2016/12",
                IlrDetails = new IlrDetails
                {
                    IlrFileName = "ILR-123456-1617-20170101-000000-01.xml",
                    FileDateTime = new DateTime(2017, 1, 1),
                    SubmittedDateTime = new DateTime(2017, 1, 1, 12, 36, 23),
                    Uln = 987654,
                    AimSeqNumber = 1,
                    StandardCode = 46,
                    ActualEndDate = new DateTime(2016, 12, 13),
                    ActualStartDate = new DateTime(2014, 12, 13),
                    PlannedEndDate = new DateTime(2018, 3, 1),
                    OnProgrammeTotalPrice = 12500,
                    CompletionTotalPrice = 3050,
                    NiNumber = "AB123456A",
                    CommitmentId = 12345L,
                    EmployerReferenceNumber = 1005,
                    EPAOrgId = "EPA0001",
                    GivenNames = "James",
                    FamilyName = "Kirk",
                    CompStatus = 1
                }
            };

            // Act
            var event2 = processor.ProcessSubmission(updatedSubmissionChanged, updatedSubmissionOriginal);

            // Assert
            Assert.AreEqual(event2.IlrFileName, updatedSubmissionChanged.IlrDetails.IlrFileName);
            Assert.AreEqual(event2.FileDateTime, updatedSubmissionChanged.IlrDetails.FileDateTime);
            Assert.AreEqual(event2.SubmittedDateTime, updatedSubmissionChanged.IlrDetails.SubmittedDateTime);
            Assert.AreEqual(event2.ComponentVersionNumber, 2);
            Assert.AreEqual(event2.UKPRN, updatedSubmissionChanged.Ukprn);
            Assert.AreEqual(event2.ULN, updatedSubmissionChanged.IlrDetails.Uln);
            Assert.AreEqual(event2.LearnRefNumber, updatedSubmissionChanged.LearnRefNumber);
            Assert.AreEqual(event2.AimSeqNumber, updatedSubmissionChanged.IlrDetails.AimSeqNumber);
            Assert.AreEqual(event2.PriceEpisodeIdentifier, updatedSubmissionChanged.PriceEpisodeIdentifier);
            Assert.AreEqual(event2.StandardCode, updatedSubmissionChanged.IlrDetails.StandardCode);
            Assert.AreEqual(event2.ProgrammeType, null);
            Assert.AreEqual(event2.FrameworkCode, null);
            Assert.AreEqual(event2.PathwayCode, null);
            Assert.AreEqual(event2.ActualStartDate, updatedSubmissionChanged.IlrDetails.ActualStartDate);
            Assert.AreEqual(event2.PlannedEndDate, updatedSubmissionChanged.IlrDetails.PlannedEndDate);
            Assert.AreEqual(event2.ActualEndDate, null);
            Assert.AreEqual(event2.OnProgrammeTotalPrice, updatedSubmissionChanged.IlrDetails.OnProgrammeTotalPrice);
            Assert.AreEqual(event2.CompletionTotalPrice, updatedSubmissionChanged.IlrDetails.CompletionTotalPrice);
            Assert.AreEqual(event2.NINumber, null);
            Assert.AreEqual(event2.GivenNames, updatedSubmissionChanged.IlrDetails.GivenNames);
            Assert.AreEqual(event2.FamilyName, updatedSubmissionChanged.IlrDetails.FamilyName);
            Assert.AreEqual(event2.CompStatus, updatedSubmissionChanged.IlrDetails.CompStatus);
            // these 3 should always be supplied, even if they haven't changed (although their change triggers an event)...
            Assert.AreEqual(event2.CommitmentId, updatedSubmissionChanged.IlrDetails.CommitmentId);
            Assert.AreEqual(event2.EPAOrgId, updatedSubmissionChanged.IlrDetails.EPAOrgId);
            Assert.AreEqual(event2.EmployerReferenceNumber, updatedSubmissionChanged.IlrDetails.EmployerReferenceNumber);
        }

        [Test]
        public void UpdatesLastSeenVersionToBeCurrentVersion()
        {
            // Arrange
            var ilrForFirstSubmission = new SubmittedPriceEpisodeEntity
            {
                Ukprn = 123456,
                LearnRefNumber = "1",
                PriceEpisodeIdentifier = "00-34-01/2016/12",
                IlrDetails = new IlrDetails
                {
                    IlrFileName = "ILR-123456-1617-20170101-000000-01.xml",
                    FileDateTime = new DateTime(2017, 1, 1),
                    SubmittedDateTime = new DateTime(2017, 1, 1, 12, 36, 23),
                    Uln = 987654,
                    AimSeqNumber = 1,
                    StandardCode = 34,
                    ActualEndDate = new DateTime(2016, 12, 13),
                    PlannedEndDate = new DateTime(2018, 2, 1),
                    OnProgrammeTotalPrice = 12000,
                    CompletionTotalPrice = 3000,
                    NiNumber = "AB123456A",
                    CommitmentId = 12345L,
                    EPAOrgId = "EPA0001",
                    GivenNames = "James",
                    FamilyName = "Kirk",
                    CompStatus = 1
                }
            };

            // Act
            var @event = processor.ProcessSubmission(ilrForFirstSubmission, null);

            // Assert
            Assert.AreEqual(@event.IlrFileName, ilrForFirstSubmission.IlrDetails.IlrFileName);
            Assert.AreEqual(@event.FileDateTime, ilrForFirstSubmission.IlrDetails.FileDateTime);
            Assert.AreEqual(@event.SubmittedDateTime, ilrForFirstSubmission.IlrDetails.SubmittedDateTime);
            Assert.AreEqual(@event.UKPRN, ilrForFirstSubmission.Ukprn);
            Assert.AreEqual(@event.ULN, ilrForFirstSubmission.IlrDetails.Uln);
            Assert.AreEqual(@event.LearnRefNumber, ilrForFirstSubmission.LearnRefNumber);
            Assert.AreEqual(@event.AimSeqNumber, ilrForFirstSubmission.IlrDetails.AimSeqNumber);
            Assert.AreEqual(@event.PriceEpisodeIdentifier, ilrForFirstSubmission.PriceEpisodeIdentifier);
            Assert.AreEqual(@event.StandardCode, ilrForFirstSubmission.IlrDetails.StandardCode);
            Assert.AreEqual(@event.ProgrammeType, ilrForFirstSubmission.IlrDetails.ProgrammeType);
            Assert.AreEqual(@event.FrameworkCode, ilrForFirstSubmission.IlrDetails.FrameworkCode);
            Assert.AreEqual(@event.PathwayCode, ilrForFirstSubmission.IlrDetails.PathwayCode);
            Assert.AreEqual(@event.ActualEndDate, ilrForFirstSubmission.IlrDetails.ActualEndDate);
            Assert.AreEqual(@event.PlannedEndDate, ilrForFirstSubmission.IlrDetails.PlannedEndDate);
            Assert.AreEqual(@event.OnProgrammeTotalPrice, ilrForFirstSubmission.IlrDetails.OnProgrammeTotalPrice);
            Assert.AreEqual(@event.CompletionTotalPrice, ilrForFirstSubmission.IlrDetails.CompletionTotalPrice);
            Assert.AreEqual(@event.NINumber, ilrForFirstSubmission.IlrDetails.NiNumber);
            Assert.AreEqual(@event.CommitmentId, ilrForFirstSubmission.IlrDetails.CommitmentId);
            Assert.AreEqual(@event.EPAOrgId, ilrForFirstSubmission.IlrDetails.EPAOrgId);
            Assert.AreEqual(@event.GivenNames, ilrForFirstSubmission.IlrDetails.GivenNames);
            Assert.AreEqual(@event.FamilyName, ilrForFirstSubmission.IlrDetails.FamilyName);
            Assert.AreEqual(@event.CompStatus, ilrForFirstSubmission.IlrDetails.CompStatus);
        }

    }
}