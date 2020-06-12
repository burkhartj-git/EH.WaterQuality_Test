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
    public class WaterShedEditViewModel
    {
        public int WaterShedID { get; set; }

        [Required(ErrorMessage = "Water Shed Description is required")]
        public string WaterShedDescription { get; set; }

        public bool ShowMessageDescription { get; set; }
        public string MessageDescription { get; set; }
    }
}