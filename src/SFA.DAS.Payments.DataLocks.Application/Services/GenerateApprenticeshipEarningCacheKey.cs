using System;
using System.Collections.Generic;
using System.Text;
using SFA.DAS.Payments.DataLocks.Domain.Infrastructure;

namespace SFA.DAS.Payments.DataLocks.Application.Services
{
    public interface IGenerateApprenticeshipEarningCacheKey
    {
        string GenerateKey(ApprenticeshipEarningCacheKeyTypes keyType, long ukprn, long uln);
    }

    public enum ApprenticeshipEarningCacheKeyTypes
    {
        Act1EarningsKey = 1,
        Act1FunctionalSkillEarningsKey,
        Act1PayableEarningsKey,
        Act1FunctionalSkillPayableEarningsKey
    }
    
    public class GenerateApprenticeshipEarningCacheKey : IGenerateApprenticeshipEarningCacheKey
    {
        public string GenerateKey(ApprenticeshipEarningCacheKeyTypes keyType, long ukprn, long uln)
        {
            return $"{Enum.GetName(keyType.GetType(), keyType)}_{ukprn}_{uln}";
        }
    }
}
