using System;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Services
{
    public class RandomUlnService : IUlnService
    {
        private const int MaximumUkprn = 1_000_000;
        private readonly Random random = new Random(Guid.NewGuid().GetHashCode());

        public long GenerateUln(long index) => random.Next(MaximumUkprn);
    }
}