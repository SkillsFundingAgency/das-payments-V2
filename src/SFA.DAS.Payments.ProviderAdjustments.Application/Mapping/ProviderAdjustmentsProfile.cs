using AutoMapper;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderAdjustments.Domain;

namespace SFA.DAS.Payments.ProviderAdjustments.Application.Mapping
{
    public class ProviderAdjustmentsProfile : Profile
    {
        public ProviderAdjustmentsProfile()
        {
            CreateMap<ProviderAdjustmentModel, ProviderAdjustment>()
                ;
        }
    }
}