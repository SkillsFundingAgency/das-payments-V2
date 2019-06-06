using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Payments.PeriodEnd.TestEndpoint.Models;

namespace SFA.DAS.Payments.PeriodEnd.TestEndpoint.Controllers
{
    public class TestEndPointController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        [HttpPost("")]
        public  IActionResult SendPeriodEnd( SendPeriodEndRequest requestModel)
        {
            return RedirectToAction(nameof(Index));
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
