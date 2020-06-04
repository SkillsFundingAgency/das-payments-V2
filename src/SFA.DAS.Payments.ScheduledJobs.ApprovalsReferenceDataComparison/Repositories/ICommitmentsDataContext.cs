using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.Payments.ScheduledJobs.ApprovalsReferenceDataComparison.Repositories.Models;

namespace SFA.DAS.Payments.ScheduledJobs.ApprovalsReferenceDataComparison.Repositories
{
    public interface ICommitmentsDataContext
    {
        DbSet<ApprenticeshipModel> Apprenticeships { get; }
    }

    public class CommitmentsDataContext : DbContext, ICommitmentsDataContext
    {
        public DbSet<ApprenticeshipModel> Apprenticeships { get; }
    }
}
