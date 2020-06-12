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
    public class SampleEditConfirmViewModel
    {
        public string RequestGroupID { get; set; }
        public string CollectedDate { get; set; }
    }
}