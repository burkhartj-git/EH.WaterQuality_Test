using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EHWaterQuality.Utilities
{
    public class EcoliData
    {
        public string TestGroupID { get; set; }
        public string SiteID { get; set; }
        public string CollectedDate { get; set; }
        public string CollectedTime { get; set; }
        public string TestName { get; set; }
        public string SampleType { get; set; }
        public string SampleMedia { get; set; }
        public string ReportedResult { get; set; }
        public string Units { get; set; }
        public string Flag { get; set; }
        public string Method { get; set; }
    }
}