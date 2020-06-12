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
    public class TestCreateGroupViewModel
    {
        public int ID { get; set; }
        public string TestDescription { get; set;}

        public List<SelectListItem> TestGroups { get; set; }
        public int SelectedTestGroup { get; set; }

        public TestCreateGroupViewModel()
        {
            TestGroups = new List<SelectListItem>();
        }
    }
}