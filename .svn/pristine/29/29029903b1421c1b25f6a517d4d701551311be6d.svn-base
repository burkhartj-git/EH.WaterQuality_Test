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
    public class ReceivingWaterGet
    {
        /// <summary>
        /// to get all receiving water locations
        /// </summary>
        public IEnumerable<SelectListItem> GetReceivingWaterLocations()
        {
            using (Entities db = new Entities())
            {
                IEnumerable<REF_WATER_BODY_TB> locationsOrdered = (IEnumerable<REF_WATER_BODY_TB>)db.REF_WATER_BODY_TB
                                                    .OrderBy(u => u.SZ_DESCRIPTION)
                                                    .ToList();

                IEnumerable<SelectListItem> locations = locationsOrdered
                                .Select(u => new SelectListItem { Value = u.N_WATER_BODY_SYSID.ToString(), Text = u.SZ_DESCRIPTION })
                                .Distinct()
                                .ToList();

                return locations;
            }
        }

        /// <summary>
        /// to get the receiving water id next value
        /// </summary>
        public int GetNextRecWaterID()
        {
            using (Entities db = new Entities())
            {
                int recWaterID = db.REF_WATER_BODY_TB.OrderByDescending(u => u.N_WATER_BODY_SYSID)
                                    .Select(u => u.N_WATER_BODY_SYSID)
                                    .FirstOrDefault();
                return recWaterID + 1;
            }
        }

        /// <summary>
        /// to check if the receiving water description exists
        /// </summary>
        public bool IsReceivingWaterDescriptionDuplicate(string ReceivingWaterDescription)
        {
            using (Entities db = new Entities())
            {
                return db.REF_WATER_BODY_TB.Any(r => r.SZ_DESCRIPTION == ReceivingWaterDescription);
            }
        }
    }
}