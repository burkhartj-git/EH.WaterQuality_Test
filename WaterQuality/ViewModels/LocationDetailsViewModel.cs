using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EHWaterQuality.Models;

namespace EHWaterQuality.ViewModels
{
    public class LocationDetailsViewModel
    {
        public int LocationID { get; set; }
        public string SearchText { get; set; }
        public int LocationGroupID { get; set; }
        public string Description { get; set; }
        public string XCoordinate { get; set; }
        public string YCoordinate { get; set; }
        public string EffectiveDate { get; set; }
        public string ExpiredDate { get; set; }
        public string Station { get; set; }
        public string OrderUpDown { get; set; }
    }
}