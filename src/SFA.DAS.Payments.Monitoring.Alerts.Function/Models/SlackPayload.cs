using System.Collections.Generic;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.Models
{
    public class SlackPayload
    {
        public string Text { get; set; }
        public List<Block> Blocks { get; set; }
    }
}
