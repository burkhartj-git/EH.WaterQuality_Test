using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EHWaterQuality.Models;
using EHWaterQuality.Utilities;

namespace EHWaterQuality.Repositories
{
    public class FacilityGet
    {
        /// <summary>
        /// to get all facilities
        /// </summary>
        public IEnumerable<SelectListItem> GetFacilities()
        {
            using (Entities db = new Entities())
            {
                IEnumerable<REF_FACILITY_TB> facilitiesOrdered = (IEnumerable<REF_FACILITY_TB>)db.REF_FACILITY_TB
                                                .OrderBy(u => u.SZ_TITLE)
                                                .ToList();

                IEnumerable<SelectListItem> facilities = facilitiesOrdered
                                .Select(u => new SelectListItem { Value = u.N_FACILITY_SYSID.ToString(), Text = u.SZ_TITLE })
                                .Distinct()
                                .ToList();

                return facilities;
            }
        }

        /// <summary>
        /// to check if the facility title exists
        /// </summary>
        public bool IsFacilityTitleDuplicate(string FacilityTitle)
        {
            using (Entities db = new Entities())
            {
                return db.REF_FACILITY_TB.Any(r => r.SZ_TITLE == FacilityTitle);
            }
        }
    }
}