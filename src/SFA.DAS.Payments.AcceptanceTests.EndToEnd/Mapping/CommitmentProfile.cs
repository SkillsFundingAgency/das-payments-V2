using AutoMapper;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Tests.Core;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Mapping
{
    public class CommitmentProfile : Profile
    {
        public CommitmentProfile()
        {
            CreateMap<Commitment, CommitmentModel>()
                .ForMember(x => x.StartDate, opt => opt.MapFrom(x => x.StartDate.ToDate()))
                .ForMember(x => x.EndDate, opt => opt.MapFrom(x => x.EndDate.ToDate()))
                ;
        }
    }
}
