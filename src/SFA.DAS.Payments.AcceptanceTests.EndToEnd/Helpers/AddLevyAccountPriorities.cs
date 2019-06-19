using System.Linq;
using SFA.DAS.Payments.AcceptanceTests.Core.Automation;
using SFA.DAS.Payments.AcceptanceTests.EndToEnd.Data;
using SFA.DAS.Payments.Application.Repositories;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.Model.Core.Entities;
using SFA.DAS.Payments.Tests.Core.Builders;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Payments.AcceptanceTests.EndToEnd.Helpers
{
    class AddLevyAccountPriorities
    {
        public static void ProcessTable(Table table, TestSession testSession, CollectionPeriod currentCollectionPeriod, IPaymentsDataContext dataContext)
        {
            var priorities = table.CreateSet<ProviderPriority>()
                .Select(x =>
                {
                    var period = new CollectionPeriodBuilder()
                        .WithSpecDate(x.SpecCollectionPeriod)
                        .Build();
                    return new 
                    {
                        AcademicYear = period.AcademicYear,
                        CollectionPeriod = period.Period,
                        Priority = x.Priority,
                        Ukprn = testSession.GetProviderByIdentifier(x.ProviderIdentifier).Ukprn,
                    };
                })
                .Where(x => x.AcademicYear == currentCollectionPeriod.AcademicYear &&
                            x.CollectionPeriod == currentCollectionPeriod.Period)
                .Select(x => new EmployerProviderPriorityModel
                {
                    EmployerAccountId = testSession.GetEmployer(null).AccountId,
                    Order = x.Priority,
                    Ukprn = x.Ukprn,
                })
                .ToList();

            if (priorities.Any())
            {
                var existingRecords = dataContext.EmployerProviderPriority.Where(x =>
                    x.EmployerAccountId == priorities.First().EmployerAccountId);
                foreach (var employerProviderPriorityModel in existingRecords)
                {
                    dataContext.EmployerProviderPriority.Remove(employerProviderPriorityModel);
                }

                dataContext.EmployerProviderPriority.AddRange(priorities);
                dataContext.SaveChanges();
            }
        }
    }
}
