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
    public class SampleBatchViewModel
    {
        public List<SelectListItem> RequestGroups { get; set; }
        public int SelectedRequestGroup { get; set; }

        [Required(ErrorMessage = "Collected Date is required")]
        [DataType(DataType.Date, ErrorMessage = "Please enter the Collected Date in the valid format")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public string CollectedDate { get; set; }

        [Required(ErrorMessage = "Collected By is required")]
        public string CollectedBy { get; set; }

        public string BatchNumber { get; set; }

        public SampleBatchViewModel()
        {
            RequestGroups = new List<SelectListItem>();
        }
    }
}