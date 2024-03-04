using System.Collections.Generic;

namespace SFA.DAS.Payments.Monitoring.Alerts.Function.Models
{
    public class Block
    {
        public string Type { get; set; }
        public BlockData Text { get; set; }
        public List<BlockData> Fields { get; set; }
    }
}
