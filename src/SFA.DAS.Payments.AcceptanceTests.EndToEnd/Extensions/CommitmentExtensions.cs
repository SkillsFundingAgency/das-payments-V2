using System.Collections.Generic;
using AutoMapper;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Extensions
{
    public static class CommitmentExtensions
    {
        public static IEnumerable<CommitmentModel> ToModel(this IEnumerable<Commitment> source)
        {
            return Mapper.Map<IEnumerable<CommitmentModel>>(source);
        }
    }
}
