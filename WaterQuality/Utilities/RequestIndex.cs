using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EHWaterQuality.Utilities
{
    public class RequestIndex
    {
        public int RequestID { get; set; }
        public string RequestGroupDescription { get; set; }
        public string LocationGroupDescription { get; set; }
        public string TestGroupDescription { get; set; }
    }
}