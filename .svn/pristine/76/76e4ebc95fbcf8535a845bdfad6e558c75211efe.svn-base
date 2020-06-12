using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EHWaterQuality.Utilities;

namespace EHWaterQuality.ViewModels
{
    public class WeatherDataSearchViewModel
    {
        public List<SelectListItem> Months {get; set;}
        public string SelectedMonth {get; set;}

        [Required(ErrorMessage = "A valid Weather Year after 1949 is required")]
        public string Year { get; set; }

        public WeatherDataSearchViewModel()
        {
            Months = new List<SelectListItem>();
            Months.Add(new SelectListItem { Value = "JAN", Text = "January" });
            Months.Add(new SelectListItem { Value = "FEB", Text = "February" });
            Months.Add(new SelectListItem { Value = "MAR", Text = "March" });
            Months.Add(new SelectListItem { Value = "APR", Text = "April" });
            Months.Add(new SelectListItem { Value = "MAY", Text = "May" });
            Months.Add(new SelectListItem { Value = "JUN", Text = "June" });
            Months.Add(new SelectListItem { Value = "JUL", Text = "July" });
            Months.Add(new SelectListItem { Value = "AUG", Text = "August" });
            Months.Add(new SelectListItem { Value = "SEP", Text = "September" });
            Months.Add(new SelectListItem { Value = "OCT", Text = "October" });
            Months.Add(new SelectListItem { Value = "NOV", Text = "November" });
            Months.Add(new SelectListItem { Value = "DEC", Text = "December" });
        }
    }
}