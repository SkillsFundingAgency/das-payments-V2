using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Payments.PeriodEnd.TestEndpoint.Model;

namespace SFA.DAS.Payments.PeriodEnd.TestEndpoint.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeriodEndMessagingController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SendPeriodEndRequest request)
        {
            return new JsonResult(
                new 
                {
                    Message = "Message sucessfully sent"
                });
        }
    }
}
