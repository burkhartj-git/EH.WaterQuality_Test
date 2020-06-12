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
    public class SewerOverflowActivityViewModel
    {
        public List<SelectListItem> Facilities {get; set;}
        public int SelectedFacility { get; set;}
        public string SearchClicked { get; set; }

        [Required(ErrorMessage = "Start Date is required")]
        [DataType(DataType.Date, ErrorMessage = "Please enter the Start Date in the valid format")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public string StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required")]
        [DataType(DataType.Date, ErrorMessage = "Please enter the End Date in the valid format")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public string EndDate { get; set; }

        public SewerOverflowActivityViewModel()
        {
            Facilities = new List<SelectListItem>();
        }
    }
}