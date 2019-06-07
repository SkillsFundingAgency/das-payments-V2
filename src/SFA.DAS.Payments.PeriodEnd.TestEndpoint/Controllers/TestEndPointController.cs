using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NServiceBus;
using SFA.DAS.Payments.Application.Messaging;
using SFA.DAS.Payments.Model.Core;
using SFA.DAS.Payments.PeriodEnd.TestEndpoint.Models;
using SFA.DAS.Payments.ProviderPayments.Messages.Internal.Commands;

namespace SFA.DAS.Payments.PeriodEnd.TestEndpoint.Controllers
{
    public class TestEndPointController : Controller
    {
        private readonly IEndpointInstanceFactory endpointInstanceFactory;

        public TestEndPointController(IEndpointInstanceFactory endpointInstanceFactory)
        {
            this.endpointInstanceFactory = endpointInstanceFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost()]
        public async Task<IActionResult> SendPeriodEnd(SendPeriodEndRequest requestModel)
        {

            var monthEndMessage = new ProcessProviderMonthEndCommand
            {
                Ukprn = requestModel.Ukprn,
                JobId = 300,
                CollectionPeriod = new CollectionPeriod
                {
                    AcademicYear = requestModel.AcademicYear,
                    Period = requestModel.Period
                },
            };

            var endpointInstance = await endpointInstanceFactory.GetEndpointInstance().ConfigureAwait(false);
            await endpointInstance.Publish(monthEndMessage).ConfigureAwait(false);

            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
