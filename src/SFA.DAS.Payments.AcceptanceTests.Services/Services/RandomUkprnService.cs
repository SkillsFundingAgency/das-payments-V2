namespace SFA.DAS.Payments.AcceptanceTests.Services
{
    using Intefaces;
    using System;

    public class RandomUkprnService : IUkprnService
    {
        private const int MaximumUkprn = 1_000_000;
        private readonly Random random = new Random(Guid.NewGuid().GetHashCode());

        public int GenerateUkprn() => random.Next(MaximumUkprn);
    }
}
