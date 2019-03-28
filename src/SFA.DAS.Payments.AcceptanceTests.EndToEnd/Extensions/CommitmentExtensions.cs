using System.Collections.Generic;
using AutoMapper;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Extensions
{
    public static class CommitmentExtensions
    {
        public static IEnumerable<ApprenticeshipModel> ToModel(this IMapper mapper, IEnumerable<Apprenticeship> source)
        {
            return mapper.Map<IEnumerable<ApprenticeshipModel>>(source);
        }
    }
}
