using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EHWaterQuality.Utilities
{
    public class LocationIndex
    {
        public int LocationID { get; set; }
        public string SiteID { get; set; }
        public string LocationDescription { get; set; }
        public string XCoordinate { get; set; }
        public string YCoordinate { get; set; }
        public string EffectiveDate { get; set; }
        public string ExpiredDate { get; set; }
        public string WaterBody { get; set; }
        public string WaterShed { get; set; }
        public string OrderUpDown { get; set; }
    }
}