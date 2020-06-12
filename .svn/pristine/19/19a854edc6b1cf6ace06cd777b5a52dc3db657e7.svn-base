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
    public class TestEditViewModel
    {
        public int TestID { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        public string SampleMedia { get; set; }

        public string SampleType { get; set; }

        public string AnalysisMethod { get; set; }

        public string Unit { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Please enter the Effective Date in the valid format")]
        public string EffectiveDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "Please enter the Expired Date in the valid format")]
        public string ExpiredDate { get; set; }
    }
}