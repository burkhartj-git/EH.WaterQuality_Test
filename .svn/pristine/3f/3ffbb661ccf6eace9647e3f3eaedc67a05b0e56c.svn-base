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
    public class RequestGroupEditViewModel
    {
        public int RequestGroupID { get; set; }

        [Required(ErrorMessage = "Request Group Description is required")]
        public string RequestGroupDescription { get; set; }

        public bool ShowMessageDescription { get; set; }
        public string MessageDescription { get; set; }
    }
}