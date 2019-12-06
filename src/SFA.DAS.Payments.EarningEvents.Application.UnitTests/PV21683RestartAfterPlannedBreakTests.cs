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
            var learning = events.First().OnProgrammeEarnings.Single(x => x.Type == OnProgrammeEarningType.Learning);
            learning.Periods[0].Amount.Should().Be(200);
            learning.Periods[1].Amount.Should().Be(200);
            learning.Periods[2].Amount.Should().Be(0);
            learning.Periods[3].Amount.Should().Be(0);
            learning.Periods[4].Amount.Should().Be(200);
            learning.Periods[5].Amount.Should().Be(200);
            learning.Periods[6].Amount.Should().Be(200);
            events.First().IncentiveEarnings.Single(x => x.Type == IncentiveEarningType.LearningSupport).Periods.Should().HaveCount(12);
            var learningSupport = events.First().IncentiveEarnings.Single(x => x.Type == IncentiveEarningType.LearningSupport);
            learningSupport.Periods[0].Amount.Should().Be(150);
            learningSupport.Periods[1].Amount.Should().Be(150);
            learningSupport.Periods[2].Amount.Should().Be(0);
            learningSupport.Periods[3].Amount.Should().Be(0);
            learningSupport.Periods[4].Amount.Should().Be(150);
            learningSupport.Periods[5].Amount.Should().Be(150);
            learningSupport.Periods[6].Amount.Should().Be(150);
        }

        [Test]
        public void RestartAfterPlannedBreakShouldHaveTwoFunctionalSkills()
        {
            var builder = new FunctionalSkillEarningEventBuilder(mapper);
            var events = builder.Build(CreateFromFile());

            events.Should().HaveCount(2);
            events.First(e => e.LearningAim.Reference.Equals("5010987X")).Earnings
                .Single(x => x.Type == FunctionalSkillType.OnProgrammeMathsAndEnglish).Periods.Should().HaveCount(12);
            var maths = events.First(e => e.LearningAim.Reference.Equals("5010987X")).Earnings.Single(x => x.Type == FunctionalSkillType.OnProgrammeMathsAndEnglish);
            maths.Periods[0].Amount.Should().Be(39.25m);
            maths.Periods[1].Amount.Should().Be(39.25m);
            maths.Periods[2].Amount.Should().Be(0);
            maths.Periods[3].Amount.Should().Be(0);
            maths.Periods[4].Amount.Should().Be(17.898m);
            maths.Periods[5].Amount.Should().Be(17.898m);
        }

        [Test]
        public void RestartAfterPlannedBreakShouldHaveTwoSubmittedLearnerAims()
        {
            var builder = new SubmittedLearnerAimBuilder(mapper);
            var events = builder.Build(CreateFromFile());

            events.Should().HaveCount(3);
            events.Where(x => x.LearningAimReference.Equals("ZPROG001")).Should().HaveCount(1);
            events.Where(x => !x.LearningAimReference.Equals("ZPROG001")).Should().HaveCount(2);
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
                        Learner = fm36Global.Learners.Single(l => l.LearnRefNumber.Equals("01LSF01BR34"))
                    };
                }
            }
        }
    }
}
