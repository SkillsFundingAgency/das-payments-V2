using AutoMapper;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Application.Mapping
{
    public class ApprenticeshipEmployerTypeResolver : IValueResolver<ApprenticeshipCreatedEvent, ApprenticeshipModel, ApprenticeshipEmployerType>
    {
        public ApprenticeshipEmployerType Resolve(ApprenticeshipCreatedEvent source, ApprenticeshipModel destination, ApprenticeshipEmployerType destMember, ResolutionContext context)
        {
            return !source.ApprenticeshipEmployerTypeOnApproval.HasValue
                ? ApprenticeshipEmployerType.Levy
                : (ApprenticeshipEmployerType) (int)source.ApprenticeshipEmployerTypeOnApproval.Value;
        }
    }
}