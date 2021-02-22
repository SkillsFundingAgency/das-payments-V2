using System;
using AutoMapper;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.ProviderPayments.Model;

namespace SFA.DAS.Payments.ProviderPayments.Application.Mapping
{
    public interface IProviderPaymentMapper
    {
        PaymentModel Map(PaymentModel payment);
    }

    public class ProviderPaymentMapper: IProviderPaymentMapper
    {
        private readonly IMapper mapper;
        public ProviderPaymentMapper(IMapper mapper)
        {
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public PaymentModel Map(PaymentModel payment)
        {
            return mapper.Map<PaymentModel>(payment);
        }
    }
}