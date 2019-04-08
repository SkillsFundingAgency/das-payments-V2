using System;
using System.Collections.Generic;
using SFA.DAS.Payments.DataLocks.Domain.Interfaces;
using SFA.DAS.Payments.DataLocks.Domain.Models;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;

namespace SFA.DAS.Payments.DataLocks.Domain.Services
{
    public class StartDateValidator : IValidator
    {
        public CourseValidationResult Validate(PriceEpisode priceEpisode, List<ApprenticeshipModel> apprenticeships)
        {
            throw new NotImplementedException();
        }
    }
}