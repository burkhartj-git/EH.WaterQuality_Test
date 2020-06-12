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
    public class RequestGroupDeleteViewModel
    {
        public int RequestGroupID { get; set; }
        public string RequestGroupDescription { get; set; }
        public bool ShowMessageDescription { get; set; }
        public string MessageDescription { get; set; }
    }
}