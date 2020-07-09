using System;
using AutoMapper;
using SFA.DAS.Payments.Model.Core.Audit;
using SFA.DAS.Payments.RequiredPayments.Messages.Events;

namespace SFA.DAS.Payments.Audit.Application.Mapping.RequiredPaymentEvents
{
    public interface IRequiredPaymentEventMapper
    {
        RequiredPaymentEventModel Map(IRequiredPaymentEvent requiredPaymentEvent);
        RequiredPaymentEventModel Map(RequiredPaymentEventModel requiredPaymentEventModel);
    }

    public class RequiredPaymentEventMapper : IRequiredPaymentEventMapper
    {
        private readonly IMapper mapper;

        public RequiredPaymentEventMapper(IMapper mapper)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public RequiredPaymentEventModel Map(IRequiredPaymentEvent requiredPaymentEvent)
        {
            return mapper.Map<RequiredPaymentEventModel>(requiredPaymentEvent);
        }

        public RequiredPaymentEventModel Map(RequiredPaymentEventModel requiredPaymentEventModel)
        {
            return mapper.Map<RequiredPaymentEventModel, RequiredPaymentEventModel>(requiredPaymentEventModel);
        }
    }
}