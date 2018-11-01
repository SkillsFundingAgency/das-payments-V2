using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using SFA.DAS.Payments.AcceptanceTests.Core.Data;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Automation
{
    public class TestSession
    {
        public LearnRefNumberGenerator LearnRefNumberGenerator { get; }
        public string SessionId { get; }
        public long Ukprn { get; }
        public List<Learner> Learners { get; }
        public Learner Learner => Learners.FirstOrDefault();
        public long JobId { get; }
        //private static ConcurrentDictionary<string, ConcurrentBag<TestSession>> Sessions { get;  } = new ConcurrentDictionary<string, ConcurrentBag<TestSession>>();  //TODO: will need to be refactored at some point
        private readonly Random random;
        private readonly Faker<Course> courseFaker;
        public TestSession()
        {
            courseFaker = new Faker<Course>();
            courseFaker
                .RuleFor(course => course.AimSeqNumber, faker => faker.Random.Short(1))
                .RuleFor(course => course.FrameworkCode, faker => faker.Random.Short(1))
                .RuleFor(course => course.FundingLineType, faker => faker.Name.JobDescriptor())
                .RuleFor(course => course.LearnAimRef, faker => faker.Random.AlphaNumeric(10))
                .RuleFor(course => course.LearningPlannedEndDate, DateTime.Today.AddMonths(12))
                .RuleFor(course => course.LearningStartDate, DateTime.Today)
                .RuleFor(course => course.PathwayCode, faker => faker.Random.Short(1))
                .RuleFor(course => course.ProgrammeType, faker => faker.Random.Short(1))
                .RuleFor(course => course.StandardCode, faker => faker.Random.Int(1))
                .RuleFor(course => course.AgreedPrice, 15000);

            SessionId = Guid.NewGuid().ToString();
            random = new Random(Guid.NewGuid().GetHashCode());
            Ukprn = GenerateId("ukprn");
            Learners = new List<Learner> { GenerateLearner() };
            JobId = GenerateId("JobId");
            LearnRefNumberGenerator = new LearnRefNumberGenerator(SessionId);
        }

        public long GenerateId(string idKey, int maxValue = 1000000)
        {
            var id = random.Next(maxValue);
            //TODO: make sure that the id isn't already in use.
            return id;
        }

        public string GenerateLearnerReference(string learnerId)
        {
            return string.IsNullOrEmpty(learnerId) ? Learner.LearnRefNumber : LearnRefNumberGenerator.Generate(Ukprn, learnerId);
        }

        public Learner GenerateLearner()
        {
            var uln = GenerateId("learner");
            return new Learner
            {
                Ukprn = Ukprn,
                Uln = uln,
                LearnRefNumber = uln.ToString(),
                Course = courseFaker.Generate(1).FirstOrDefault()
            };
        }

        public void SessionEnd()
        {
            //TODO: clean up Ids
        }
    }
}
