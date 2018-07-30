using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Payments.PaymentsDue.Domain.Entities;
using SFA.DAS.Payments.PaymentsDue.Domain.Models;

namespace SFA.DAS.Payments.PaymentsDue.Application.Repositories
{
    public interface IApprenticeshipRepository
    {
        Task<IApprenticeship> GetApprenticeship(Learner learner);
    }
}
