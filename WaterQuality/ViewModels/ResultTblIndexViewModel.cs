using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EHWaterQuality.ViewModels
{
    public class ResultTblIndexViewModel
    {
        public string ResultID { get; set; }
        public string ResultValue { get; set; }
        public string ResultValueIndicator { get; set; }
        public string Test { get; set; }
        public string SampleID { get; set; }
    }
}