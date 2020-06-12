using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EHWaterQuality.Models;

namespace EHWaterQuality.ViewModels
{
    public class WaterBodyIndexViewModel
    {
        public int ID { get; set; }
        public string WaterBodyName { get; set; }
        public string WaterBodyDescription { get; set; }
    }
}