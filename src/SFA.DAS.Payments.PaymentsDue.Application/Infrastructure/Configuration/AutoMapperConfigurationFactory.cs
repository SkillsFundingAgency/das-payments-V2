using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using SFA.DAS.Payments.PaymentsDue.Domain.Entities;
using SFA.DAS.Payments.PaymentsDue.Model.Entities;

namespace SFA.DAS.Payments.PaymentsDue.Application.Infrastructure.Configuration
{
    public class AutoMapperConfigurationFactory
    {
        public static MapperConfiguration CreateMappingConfig()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PaymentEntity, Payment>();
            });
        }
    }
}
