using System.IO;
using System.Linq;
using System.Reflection;
using AutoMapper;
using ESFA.DC.ILR.FundingService.FM36.FundingOutput.Model.Output;
using FluentAssertions;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Payments.EarningEvents.Application.Mapping;
using SFA.DAS.Payments.EarningEvents.Messages.Internal.Commands;
using SFA.DAS.Payments.Model.Core.Incentives;
using SFA.DAS.Payments.Model.Core.OnProgramme;

namespace SFA.DAS.Payments.EarningEvents.Application.UnitTests
{
    [TestFixture]
    public class Pv21683RestartAfterPlannedBreakTests
    {
        private IMapper mapper;

        [OneTimeSetUp]
        public void InitialiseMapper()
        {
            mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<EarningsEventProfile>()));
        }

        [Test]
        public void RestartAfterPlannedBreakShouldHaveOneMainAim()
        {
            var builder = new ApprenticeshipContractTypeEarningsEventBuilder(new ApprenticeshipContractTypeEarningsEventFactory(), mapper);
            var events = builder.Build(CreateFromFile());

            events.Should().HaveCount(1);
            events.First().PriceEpisodes.Should().HaveCount(2);
            events.First().OnProgrammeEarnings.Single(x => x.Type == OnProgrammeEarningType.Learning).Periods.Should().HaveCount(12);
        }

        [Test]
        public void RestartAfterPlannedBreakShouldHaveTwoFunctionalSkills()
        {
            var builder = new FunctionalSkillEarningEventBuilder(mapper);
            var events = builder.Build(CreateFromFile());

            events.Should().HaveCount(2);
            events.First().Earnings.Single(x => x.Type == FunctionalSkillType.OnProgrammeMathsAndEnglish).Periods.Should().HaveCount(12);
        }

        [Test]
        public void RestartAfterPlannedBreakShouldHaveTwoSubmittedLearnerAims()
        {
            var builder = new SubmittedLearnerAimBuilder(mapper);
            var events = builder.Build(CreateFromFile());

            events.Should().HaveCount(3);

        }

        private ProcessLearnerCommand CreateFromFile()
        {
            var embeddedProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly());
            using (var reader = embeddedProvider.GetFileInfo("DataFiles\\PV21683_FM36OutputOnRestart.json").CreateReadStream())
            {
                using (var sr = new StreamReader(reader))
                {
                    var fm36Global = JsonConvert.DeserializeObject<FM36Global>(sr.ReadToEnd());
                    return new ProcessLearnerCommand
                    {
                        CollectionPeriod = 3,
                        CollectionYear = short.Parse(fm36Global.Year),
                        Ukprn = fm36Global.UKPRN,
                        Learner = fm36Global.Learners.Single(l => l.LearnRefNumber.Equals("9000005107"))
                    };
                }
            }
        }
    }
}
