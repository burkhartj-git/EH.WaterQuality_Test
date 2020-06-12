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
    public class LocationEditViewModel
    {
        public int LocationID { get; set; }

        public string SearchText { get; set; }

        [Required(ErrorMessage = "Station # is required")]
        public string Station { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Numeric(ErrorMessage = "XCoordinate must be numeric")]
        public double XCoordinate { get; set; }

        [Numeric(ErrorMessage = "YCoordinate must be numeric")]
        public double YCoordinate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Please enter the Effective Date in the valid format")]
        public string EffectiveDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Please enter the Expired Date in the valid format")]
        public string ExpiredDate { get; set; }

        public List<SelectListItem> WaterBodies { get; set; }
        //[Required(ErrorMessage = "Please select a Water Body")]
        public int? SelectedWaterBody { get; set; }

        public List<SelectListItem> WaterSheds { get; set; }
        //[Required(ErrorMessage = "Please select a Water Shed")]
        public int? SelectedWaterShed { get; set; }

        public string OrderUpDown { get; set; }

        public LocationEditViewModel()
        {
            WaterBodies = new List<SelectListItem>();
            WaterSheds = new List<SelectListItem>();
        }
    }
}