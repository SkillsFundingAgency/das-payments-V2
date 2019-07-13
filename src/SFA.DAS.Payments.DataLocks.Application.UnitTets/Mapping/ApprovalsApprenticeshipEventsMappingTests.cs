using System;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.CommitmentsV2.Types;
using SFA.DAS.Payments.DataLocks.Application.Mapping;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Application.UnitTests.Mapping
{
    [TestFixture]
    public class ApprovalsApprenticeshipEventsMappingTests
    {
        protected IMapper Mapper { get; private set; }

        [OneTimeSetUp]
        public void InitialiseMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<DataLocksProfile>());
            Mapper = new Mapper(config);
        }

        [Test]
        public void Maps_ApprenticeshipCreatedEvent_To_ApprenticeshipModel()
        {
            var approvalsEvent = new ApprenticeshipCreatedEvent
            {
                AccountId = 12345,
                StartDate = DateTime.Today.AddMonths(-12),
                AccountLegalEntityPublicHashedId = "1234567890",
                AgreedOn = DateTime.Today.AddMonths(-13),
                ApprenticeshipId = 12,
                CreatedOn = DateTime.Today.AddMonths(-14),
                EndDate = DateTime.Today.AddYears(1),
                LegalEntityName = "Test Employer",
                ProviderId = 1234,
                TrainingCode = "52",
                TrainingType = ProgrammeType.Standard,
                TransferSenderId = 123456,
                Uln = "123456",
                PriceEpisodes = new PriceEpisode[]
                {
                    new PriceEpisode { FromDate = DateTime.Today.AddMonths(-12), Cost = 1000M, ToDate = DateTime.Today.AddDays(-1)},
                    new PriceEpisode { FromDate = DateTime.Today, Cost = 1200M }
                }
            };
            var model = Mapper.Map<ApprenticeshipModel>(approvalsEvent);
            model.AccountId.Should().Be(approvalsEvent.AccountId);
            model.AgreedOnDate.Should().Be(approvalsEvent.AgreedOn);
            model.AgreementId.Should().Be(approvalsEvent.AccountLegalEntityPublicHashedId);
            model.EstimatedEndDate.Should().Be(approvalsEvent.EndDate);
            model.EstimatedStartDate.Should().Be(approvalsEvent.StartDate);
            model.StandardCode.ToString().Should().Be(approvalsEvent.TrainingCode);
            model.Id.Should().Be(approvalsEvent.ApprenticeshipId);
            model.LegalEntityName.Should().Be(approvalsEvent.LegalEntityName);
            model.Status.Should().Be(ApprenticeshipStatus.Active);
            model.StopDate.Should().BeNull();
            model.TransferSendingEmployerAccountId.Should().Be(approvalsEvent.TransferSenderId);
            model.Ukprn.Should().Be(approvalsEvent.ProviderId);
            model.Uln.ToString().Should().Be(approvalsEvent.Uln);
            model.ApprenticeshipPriceEpisodes.Count.Should().Be(approvalsEvent.PriceEpisodes.Length);
            approvalsEvent.PriceEpisodes.All(pe => model.ApprenticeshipPriceEpisodes.Any(ape =>
                ape.StartDate == pe.FromDate && ape.Cost == pe.Cost && ape.EndDate == pe.ToDate)).Should().BeTrue();

        }

        [Test]
        public void Defaults_Programme_Type_To_25_For_Standards()
        {
            var approvalsEvent = new ApprenticeshipCreatedEvent
            {
                AccountId = 12345,
                StartDate = DateTime.Today.AddMonths(-12),
                AccountLegalEntityPublicHashedId = "1234567890",
                AgreedOn = DateTime.Today.AddMonths(-13),
                ApprenticeshipId = 12,
                CreatedOn = DateTime.Today.AddMonths(-14),
                EndDate = DateTime.Today.AddYears(1),
                LegalEntityName = "Test Employer",
                ProviderId = 1234,
                TrainingCode = "52",
                TrainingType = ProgrammeType.Standard,
                TransferSenderId = 123456,
                Uln = "123456",
                PriceEpisodes = new PriceEpisode[]
                {
                    new PriceEpisode { FromDate = DateTime.Today.AddMonths(-12), Cost = 1000M, ToDate = DateTime.Today.AddDays(-1)},
                    new PriceEpisode { FromDate = DateTime.Today, Cost = 1200M }
                }
            };
            var model = Mapper.Map<ApprenticeshipModel>(approvalsEvent);
            model.ProgrammeType.Should().Be(25);
        }

        [Test]
        public void Parses_TrainingCode_Correctly_For_Frameworks()
        {
            var approvalsEvent = new ApprenticeshipCreatedEvent
            {
                AccountId = 12345,
                StartDate = DateTime.Today.AddMonths(-12),
                AccountLegalEntityPublicHashedId = "1234567890",
                AgreedOn = DateTime.Today.AddMonths(-13),
                ApprenticeshipId = 12,
                CreatedOn = DateTime.Today.AddMonths(-14),
                EndDate = DateTime.Today.AddYears(1),
                LegalEntityName = "Test Employer",
                ProviderId = 1234,
                TrainingCode = "460-3-2",
                TrainingType = ProgrammeType.Framework,
                TransferSenderId = 123456,
                Uln = "123456",
                PriceEpisodes = new PriceEpisode[]
                {
                    new PriceEpisode { FromDate = DateTime.Today.AddMonths(-12), Cost = 1000M, ToDate = DateTime.Today.AddDays(-1)},
                    new PriceEpisode { FromDate = DateTime.Today, Cost = 1200M }
                }
            };
            var model = Mapper.Map<ApprenticeshipModel>(approvalsEvent);
            model.ProgrammeType.Should().Be(3);
            model.FrameworkCode.Should().Be(460);
            model.PathwayCode.Should().Be(2);
        }

        [Test]
        public void Apprenticeship_Created_Defaults_Is_Levy_Payer_To_True()
        {
            var approvalsEvent = new ApprenticeshipCreatedEvent
            {
                AccountId = 12345,
                StartDate = DateTime.Today.AddMonths(-12),
                AccountLegalEntityPublicHashedId = "1234567890",
                AgreedOn = DateTime.Today.AddMonths(-13),
                ApprenticeshipId = 12,
                CreatedOn = DateTime.Today.AddMonths(-14),
                EndDate = DateTime.Today.AddYears(1),
                LegalEntityName = "Test Employer",
                ProviderId = 1234,
                TrainingCode = "460-3-2",
                TrainingType = ProgrammeType.Framework,
                TransferSenderId = 123456,
                Uln = "123456",
                PriceEpisodes = new PriceEpisode[]
                {
                    new PriceEpisode { FromDate = DateTime.Today.AddMonths(-12), Cost = 1000M, ToDate = DateTime.Today.AddDays(-1)},
                    new PriceEpisode { FromDate = DateTime.Today, Cost = 1200M }
                }
            };
            var model = Mapper.Map<ApprenticeshipModel>(approvalsEvent);
            model.IsLevyPayer.Should().BeTrue();
        }


        [Test]
        public void Maps_ApprenticeshipUpdatedApprovedEvent_To_UpdatedApprenticeshipModel_Correctly()
        {
            var approvalsEvent = new ApprenticeshipUpdatedApprovedEvent
            {
                StartDate = DateTime.Today.AddMonths(-12),
                ApprovedOn = DateTime.Today.AddMonths(-13),
                ApprenticeshipId = 12,
                EndDate = DateTime.Today.AddYears(1),
                TrainingCode = "460-3-2",
                TrainingType = ProgrammeType.Framework,
                Uln = "123456",
                PriceEpisodes = new PriceEpisode[]
                {
                    new PriceEpisode { FromDate = DateTime.Today.AddMonths(-12), Cost = 1000M, ToDate = DateTime.Today.AddDays(-1)},
                    new PriceEpisode { FromDate = DateTime.Today, Cost = 1200M }
                }
            };
            var model = Mapper.Map<UpdatedApprenticeshipModel>(approvalsEvent);

            model.AgreedOnDate.Should().Be(approvalsEvent.ApprovedOn);
            model.EstimatedEndDate.Should().Be(approvalsEvent.EndDate);
            model.EstimatedStartDate.Should().Be(approvalsEvent.StartDate);
            model.ApprenticeshipId.Should().Be(approvalsEvent.ApprenticeshipId);
            model.StandardCode.Should().Be(0);
            model.FrameworkCode.Should().Be(460);
            model.ProgrammeType.Should().Be(3);
            model.PathwayCode.Should().Be(2);
            model.Uln.ToString().Should().Be(approvalsEvent.Uln);
            model.ApprenticeshipPriceEpisodes.Count.Should().Be(approvalsEvent.PriceEpisodes.Length);
            approvalsEvent.PriceEpisodes
                .All(pe => model.ApprenticeshipPriceEpisodes.Any(ape => ape.StartDate == pe.FromDate && ape.Cost == pe.Cost && ape.EndDate == pe.ToDate))
                .Should().BeTrue();
        }

    }
}