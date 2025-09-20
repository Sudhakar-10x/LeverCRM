using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _10xErp.Models
{
    public class ActivityViewModel
    {
        public int ActivityID { get; set; }
        public string Date { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
        public string CustomerCode { get; set; }
        public string Subject { get; set; }
        public string Details { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public string SalesEmployee { get; set; }
    }
}