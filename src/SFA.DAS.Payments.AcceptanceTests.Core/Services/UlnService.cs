using System;
using System.Collections.Generic;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Services
{
    public class UlnService : IUlnService
    {
        private static readonly HashSet<long> usedIndices = new HashSet<long>();

        public long GenerateUln(long index)
        {
            if (usedIndices.Contains(index))
                index = index + (index - 10_000_000);
            else
                usedIndices.Add(index);

            int roundedIndex = RoundUpToMultipleOfTen(index);
            try
            {
                return ComputeUln(roundedIndex);
            }
            catch (ArgumentOutOfRangeException)
            {
                return ComputeUln(roundedIndex + 10);
            }
        }

        private int RoundUpToMultipleOfTen(long index)
        {
            var normalisedValue = index;
            if (normalisedValue >= 10000000)
            {
                normalisedValue = index - 10000000;
            }

            return (int)(Math.Ceiling((normalisedValue) / 10d) * 10);
        }

        private static long ComputeUln(long index)
        {
            // this black magic is borrowed from TDG, don't touch it. It works.

            index += 990000000;
            string s = index.ToString();
            s = s.PadRight(9, '0');
            long result = 0;
            long multiplier = 10;
            for (int i = 0; i != s.Length; ++i)
            {
                result += multiplier-- * (s[i] - '0');
            }

            long mod11 = result % 11;
            if (mod11 == 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            long check = 10 - mod11;
            s += check.ToString();

            return long.Parse(s);
        }
    }
}