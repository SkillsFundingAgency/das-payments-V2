using System;
using AutoMapper;
using SFA.DAS.Payments.FundingSource.Messages.Events;
using SFA.DAS.Payments.Model.Core.Audit;

namespace SFA.DAS.Payments.Audit.Application.Mapping.FundingSource
{
    public interface IFundingSourceEventMapper
    {
        FundingSourceEventModel Map(IFundingSourcePaymentEvent fundingSourceEvent);
        FundingSourceEventModel Map(FundingSourceEventModel model);
    }

    public class FundingSourceEventMapper : IFundingSourceEventMapper
    {
        private readonly IMapper mapper;

        public FundingSourceEventMapper(IMapper mapper)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public FundingSourceEventModel Map(IFundingSourcePaymentEvent fundingSourceEvent)
        {
            return mapper.Map<FundingSourceEventModel>(fundingSourceEvent);
        }

        public FundingSourceEventModel Map(FundingSourceEventModel model)
        {
            return mapper.Map<FundingSourceEventModel, FundingSourceEventModel>(model);
        }
    }
}