using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinyUrlApp.CornJob.Model
{
    public class CronJobResult
    {
        public bool IsSuccess { get; set; }
        public int StatusCode { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime TriggeredAt { get; set; }
    }
}
