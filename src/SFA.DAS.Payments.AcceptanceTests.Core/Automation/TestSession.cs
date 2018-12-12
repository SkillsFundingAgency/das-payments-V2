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
        public long Ukprn { get; private set; }
        public List<Learner> Learners { get; }
        public Learner Learner => Learners.FirstOrDefault();
        public long JobId { get; }
        public DateTime IlrSubmissionTime { get; }
        //private static ConcurrentDictionary<string, ConcurrentBag<TestSession>> Sessions { get;  } = new ConcurrentDictionary<string, ConcurrentBag<TestSession>>();  //TODO: will need to be refactored at some point
        private readonly Random random;
        private readonly Faker<Course> courseFaker;
        public TestSession()
        {
            courseFaker = new Faker<Course>();
            courseFaker
                .RuleFor(course => course.AimSeqNumber, faker => faker.Random.Short(1))
                .RuleFor(course => course.FrameworkCode, faker => faker.Random.Short(1))
                .RuleFor(course => course.FundingLineType, faker => faker.Name.JobDescriptor()?? "FundingLine")
                .RuleFor(course => course.LearnAimRef, "ZPROG001")
                .RuleFor(course => course.LearningPlannedEndDate, DateTime.Today.AddMonths(12))
                .RuleFor(course => course.LearningStartDate, DateTime.Today)
                .RuleFor(course => course.PathwayCode, faker => faker.Random.Short(1))
                .RuleFor(course => course.ProgrammeType, faker => faker.Random.Short(1))
                .RuleFor(course => course.StandardCode, faker => faker.Random.Int(1))
                .RuleFor(course => course.AgreedPrice, 15000);

            SessionId = Guid.NewGuid().ToString();
            random = new Random(Guid.NewGuid().GetHashCode());
            Ukprn = GenerateId();
            Learners = new List<Learner> { GenerateLearner() };
            JobId = GenerateId();
            LearnRefNumberGenerator = new LearnRefNumberGenerator(SessionId);
            IlrSubmissionTime = DateTime.UtcNow;
        }

        public long GenerateId(int maxValue = 1000000)
        {
            var id = random.Next(maxValue);
            //TODO: make sure that the id isn't already in use.
            return id;
        }

        public void RegenerateUkprn()
        {
            Ukprn = GenerateId();
        }

        public string GenerateLearnerReference(string learnerId)
        {
            return string.IsNullOrEmpty(learnerId) ? Learner.LearnRefNumber : LearnRefNumberGenerator.Generate(Ukprn, learnerId);
        }

        public Learner GenerateLearner()
        {
            var uln = GenerateId();
            return new Learner
            {
                Ukprn = Ukprn,
                Uln = uln,
                LearnRefNumber = uln.ToString(),
                Course = courseFaker.Generate(1).FirstOrDefault()
            };
        }

        public Learner GetLearner(string learnerIdentifier)
        {
            return Learners.FirstOrDefault(l => l.LearnerIdentifier == learnerIdentifier) ??
                   throw new ArgumentException($"Learner with identifier: '{learnerIdentifier}' not found.");
        }

        public void SessionEnd()
        {
            //TODO: clean up Ids
        }
    }
}