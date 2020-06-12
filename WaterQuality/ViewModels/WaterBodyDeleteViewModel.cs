using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EHWaterQuality.ViewModels
{
    public class WaterBodyDeleteViewModel
    {
        public int WaterBodyID { get; set; }
        public string WaterBodyName { get; set; }
        public string WaterBodyDescription { get; set; }
        public bool ShowMessageNameAndDescription { get; set; }
        public string MessageNameAndDescription { get; set; }
    }
}