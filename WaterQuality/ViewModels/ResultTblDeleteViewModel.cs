using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EHWaterQuality.ViewModels
{
    public class ResultTblDeleteViewModel
    {
        public int ID { get; set; }
        public bool ShowMessage { get; set; }
        public string Message { get; set; }
    }
}