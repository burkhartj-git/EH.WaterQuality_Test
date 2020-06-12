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
    public class RequestDeleteViewModel
    {
        public int RequestID { get; set; }
        public string RequestGroup { get; set;}
        public int SelectedRequestGroup { get; set; }
        public string LocationGroup { get; set; }
        public int SelectedLocationGroup { get; set; }
        public string TestGroup { get; set; }
        public int SelectedTestGroup { get; set; }
        public bool ShowMessage { get; set; }
        public string Message { get; set; }
    }
}