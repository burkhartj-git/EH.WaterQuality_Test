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
    public class TestGroupEditViewModel
    {
        public int TestGroupID { get; set; }

        [Required(ErrorMessage = "Test Group Description is required")]
        public string TestGroupDescription { get; set; }

        public bool ShowMessageDescription { get; set; }
        public string MessageDescription { get; set; }
    }
}