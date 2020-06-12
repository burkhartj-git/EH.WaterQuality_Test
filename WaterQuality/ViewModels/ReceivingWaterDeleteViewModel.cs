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
    public class ReceivingWaterDeleteViewModel
    {
        public int ReceivingWaterID { get; set; }
        public string ReceivingWaterDescription { get; set; }
        public bool ShowMessageDescription { get; set; }
        public string MessageDescription { get; set; }
    }
}