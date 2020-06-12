using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EHWaterQuality.Models
{
    public class TimeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            bool isValid = true;

            if (value == null) return true;

            int i;
            isValid = int.TryParse(value.ToString(), out i) && value.ToString().Length == 4;

            return isValid;
        }
    }
}