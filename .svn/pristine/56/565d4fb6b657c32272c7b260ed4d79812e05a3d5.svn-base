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
    public class TestGroupCreateTestViewModel
    {
        public int ID { get; set; }
        public string TestGroupDescription { get; set; }

        public List<SelectListItem> Tests { get; set; }
        public int SelectedTest { get; set; }

        public TestGroupCreateTestViewModel()
        {
            Tests = new List<SelectListItem>();
        }
    }
}