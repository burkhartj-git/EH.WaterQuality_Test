using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EHWaterQuality.Utilities
{
    public class TestIndex
    {
        public int TestID { get; set; }
        public string TestName { get; set; }
        public string Units { get; set; }
        public string SampleMedia { get; set; }
        public string SampleType { get; set; }
        public string AMethod { get; set; }
        public string EffectiveDate { get; set; }
        public string ExpiredDate { get; set; }
    }
}