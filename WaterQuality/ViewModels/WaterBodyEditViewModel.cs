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
    public class WaterBodyEditViewModel
    {
        public int WaterBodyID { get; set; }

        [Required(ErrorMessage = "Water Body Name is required")]
        public string WaterBodyName { get; set; }

        [Required(ErrorMessage = "Water Body Description is required")]
        public string WaterBodyDescription { get; set; }

        public bool ShowMessageNameAndDescription { get; set; }
        public string MessageNameAndDescription { get; set; }
    }
}