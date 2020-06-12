using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;

namespace EHWaterQuality.Models
{
    public class StringDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            bool isValid = true;

            if (value == null) return true;

            DateTime parsedDate;

            isValid = DateTime.TryParseExact(value.ToString(), "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate);

            return isValid;
        }
    }
}