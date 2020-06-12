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
    public class LocationGroupCreateLocationViewModel
    {
        public int LocationGroupID { get; set; }
        public string LocationGroupDescription { get; set; }

        public List<SelectListItem> Locations { get; set; }
        public int SelectedLocation { get; set; }

        public LocationGroupCreateLocationViewModel()
        {
            Locations = new List<SelectListItem>();
        }
    }
}