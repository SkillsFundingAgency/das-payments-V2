using System.Collections.Generic;
using AutoMapper;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Extensions
{
    public static class LevyAccountExtensions
    {
        public static IEnumerable<LevyAccountModel> ToModel(this IEnumerable<LevyAccount> source)
        {
            return Mapper.Map<IEnumerable<LevyAccountModel>>(source);
        }
    }
}
