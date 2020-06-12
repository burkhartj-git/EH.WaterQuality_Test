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
    public class ReceivingWaterEditViewModel
    {
        public int ReceivingWaterID { get; set;}

        [Required(ErrorMessage = "Receiving Water Description is required")]
        public string ReceivingWaterDescription { get; set;}

        public bool ShowMessageDescription { get; set; }
        public string MessageDescription { get; set; }
    }
}