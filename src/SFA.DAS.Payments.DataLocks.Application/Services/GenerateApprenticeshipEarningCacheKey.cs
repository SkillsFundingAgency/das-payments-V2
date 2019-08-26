using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.Payments.DataLocks.Domain.Infrastructure;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public interface IGenerateApprenticeshipEarningCacheKey
    {
        string GenerateAct1EarningsKey(long ukprn, long uln);
        string GenerateAct1FunctionalSkillEarningsKey(long ukprn, long uln);
    }

    public class GenerateApprenticeshipEarningCacheKey : IGenerateApprenticeshipEarningCacheKey
    {
        public string GenerateAct1EarningsKey(long ukprn, long uln)
        {
            return $"Act1EarningsKey_{ukprn}_{uln}";
        }

        public string GenerateAct1FunctionalSkillEarningsKey(long ukprn, long uln)
        {
            return $"Act1FunctionalSkillEarningsKey_{ukprn}_{uln}";
        }
    }
}
