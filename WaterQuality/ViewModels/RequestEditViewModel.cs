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
    public class RequestEditViewModel
    {
        public int RequestID { get; set; }

        public List<SelectListItem> RequestGroups { get; set; }
        [Required(ErrorMessage = "Please select a Request Group")]
        public int SelectedRequestGroup { get; set; }

        public List<SelectListItem> LocationGroups { get; set; }
        [Required(ErrorMessage = "Please select a Location Group")]
        public int SelectedLocationGroup { get; set; }

        public List<SelectListItem> TestGroups { get; set; }
        [Required(ErrorMessage = "Please select a Test Group")]
        public int SelectedTestGroup { get; set; }

        public RequestEditViewModel()
        {
            RequestGroups = new List<SelectListItem>();
            LocationGroups = new List<SelectListItem>();
            TestGroups = new List<SelectListItem>();
        }
    }
}