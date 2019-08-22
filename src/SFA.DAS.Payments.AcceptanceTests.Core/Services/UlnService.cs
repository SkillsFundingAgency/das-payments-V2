using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Payments.AcceptanceTests.Core.Services
{
    public class UlnService : IUlnService
    {
        private static readonly Dictionary<long, List<(string identifier, long uln)>> UsedIndices =
            new Dictionary<long, List<(string, long)>>();

        public long GenerateUln(long ukPrn, string identifier)
        {
            var index = ukPrn;
            if (UsedIndices.ContainsKey(index))
            {
                var learners = UsedIndices[index];
                if (learners.Any(x => x.identifier == identifier))
                {
                    return learners.Single(l => l.identifier == identifier).uln;
                }

                index += (index % 10_000_000) * learners.Count;
                var computedUln = GenerateUln(index);
                learners.Add((identifier, computedUln));
                return computedUln;
            }
            else
            {
                var computedUln = GenerateUln(index);
                var learners = new List<(string, long)> { (identifier, computedUln) };
                UsedIndices.Add(ukPrn, learners);
                return computedUln;
            }
        }

        private long GenerateUln(long index)
        {
            var roundedIndex = RoundUpToMultipleOfTen(index);
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
            var normalisedValue = index % 10_000_000;
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