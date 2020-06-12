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
    public class LocationCreateGroupViewModel
    {
        public int ID { get; set; }
        public string LocationDescription { get; set; }
        public string SearchText { get; set; }

        public List<SelectListItem> LocationGroups { get; set; }
        public int SelectedLocationGroup { get; set; }

        public LocationCreateGroupViewModel()
        {
            LocationGroups = new List<SelectListItem>();
        }
    }
}