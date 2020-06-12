using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EHWaterQuality.ViewModels
{
    public class FacilityEditViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Facility Title is required")]
        public string Title { get; set; }

        public bool ShowMessageTitle { get; set; }
        public string MessageTitle { get; set; }
    }
}