using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EHWaterQuality.Models;

namespace EHWaterQuality.ViewModels
{
    public class SewerOverflowActivityEditViewModel
    {
        public int SewerLogID { get; set; }

        public string FacilityID { get; set; }
        public string SearchStartDate { get; set; }
        public string SearchEndDate { get; set; }

        public List<SelectListItem> Facilities {get; set; }
        [Required(ErrorMessage = "Please select a facility")]
        public int SelectedFacility { get; set; }

        [Required(ErrorMessage = "Start Date is required")]
        [StringDate(ErrorMessage = "Please enter the Start Date in the valid format")]
        public string StartDate { get; set; }

        //[Required(ErrorMessage = "End Date is required")]
        [StringDate(ErrorMessage = "Please enter the End Date in the valid format")]
        public string EndDate { get; set; }

        //[RegularExpression(@"^(\d{4})$", ErrorMessage = "Please enter a 4 digit number for Start Time")]
        [Time(ErrorMessage = "Please enter the StartTime in the correct format")]
        public string StartTime { get; set; }

        //[RegularExpression(@"^(\d{4})$", ErrorMessage = "Please enter a 4 digit number for End Time")]
        [Time(ErrorMessage = "Please enter the EndTime in the correct format")]
        public string EndTime { get; set; }

        [Numeric(ErrorMessage = "Please enter a number for the Duration")]
        public string Duration { get; set; }

        [Numeric(ErrorMessage = "Please enter a number for the Discharge Gallons")]
        public string DischargeGallons { get; set; }

        [Numeric(ErrorMessage = "Please enter a number for the Precipitation")]
        public string Precipitation { get; set; }

        public List<SelectListItem> ReceivingWaters { get; set; }
        [Required(ErrorMessage = "Please select a receiving water")]
        public int SelectedReceivingWater { get; set; }

        public List<SelectListItem> Chlorinates { get; set; }
        //[Required(ErrorMessage = "Please select a chlorinate")]
        public string SelectedChlorinate { get; set; }

        public List<SelectListItem> NPDESs { get; set; }
        //[Required(ErrorMessage = "Please select a NPDES")]
        public string SelectedNPDES { get; set; }

        public List<SelectListItem> ActivityTypes { get; set; }
        //[Required(ErrorMessage = "Please select an Activity Type")]
        public string SelectedActivityType { get; set; }

        public SewerOverflowActivityEditViewModel()
        {
            Facilities = new List<SelectListItem>();
            ReceivingWaters = new List<SelectListItem>();
            Chlorinates = new List<SelectListItem>();
            NPDESs = new List<SelectListItem>();
            ActivityTypes = new List<SelectListItem>();
        }
    }
}