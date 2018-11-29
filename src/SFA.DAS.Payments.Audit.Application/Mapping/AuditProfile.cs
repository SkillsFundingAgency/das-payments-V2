using AutoMapper;
using SFA.DAS.Payments.Audit.Model;
using SFA.DAS.Payments.Messages.Core.Events;

namespace SFA.DAS.Payments.Audit.Application.Mapping
{
    public class AuditProfile: Profile
    {
        public AuditProfile()
        {
            CreateMap<IPeriodisedPaymentEvent,PeriodisedModel>()
        }
    }
}