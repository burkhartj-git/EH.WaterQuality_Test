using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EHWaterQuality.Models;

namespace EHWaterQuality.ViewModels
{
    public class RequestIndexViewModel
    {
        public int ID { get; set; }
        public string RequestGroupDescription { get; set; }
        public string LocationGroupDescription { get; set; }
        public string TestGroupDescription { get; set; }
    }
}