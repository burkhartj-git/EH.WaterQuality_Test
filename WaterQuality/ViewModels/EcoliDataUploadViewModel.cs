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
    public class EcoliDataUploadViewModel
    {
        public List<SelectListItem> EcoliTestTypes { get; set; }
        public int SelectedEcoliTestType { get; set; }
        public HttpPostedFileBase File { get; set; }
    }
}