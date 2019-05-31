using System;

namespace SFA.DAS.Payments.AcceptanceTests.Core.TestModels
{
    public class Provider
    {
        public int Ukprn { get; private set;  }

        public DateTime LastUsed { get; private set; }

        internal void Use() => LastUsed = DateTime.UtcNow;
    }
}
