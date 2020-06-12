using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EHWaterQuality.Models;
using EHWaterQuality.Utilities;

namespace EHWaterQuality.Repositories
{
    public class WaterShedGet
    {
        /// <summary>
        /// to get all water sheds
        /// </summary>
        public IEnumerable<SelectListItem> GetWaterSheds()
        {
            using (Entities db = new Entities())
            {
                IEnumerable<REF_LOCATION_TYPE_TB> waterShedsOrdered = (IEnumerable<REF_LOCATION_TYPE_TB>)db.REF_LOCATION_TYPE_TB
                                                .OrderBy(u => u.SZ_DESCRIPTION)
                                                .ToList();

                IEnumerable<SelectListItem> waterSheds = waterShedsOrdered
                                .Select(u => new SelectListItem { Value = u.N_LOCATION_TYPE_SYSID.ToString(), Text = u.SZ_DESCRIPTION })
                                .Distinct()
                                .ToList();

                return waterSheds;
            }
        }

        /// <summary>
        /// to get the water shed id next value
        /// </summary>
        public int GetNextWaterShedID()
        {
            using (Entities db = new Entities())
            {
                int WaterShedID = db.REF_LOCATION_TYPE_TB.OrderByDescending(u => u.N_LOCATION_TYPE_SYSID)
                                    .Select(u => u.N_LOCATION_TYPE_SYSID)
                                    .FirstOrDefault();
                return WaterShedID + 1;
            }
        }

        /// <summary>
        /// to check if the water shed description exists
        /// </summary>
        public bool IsWaterShedDescriptionDuplicate(string WaterShedDescription)
        {
            using (Entities db = new Entities())
            {
                return db.REF_LOCATION_TYPE_TB.Any(r => r.SZ_DESCRIPTION == WaterShedDescription);
            }
        }

        /// <summary>
        /// to get water shed description by id
        /// </summary>
        public string GetWaterShedDescriptionByID(int ID)
        {
            using (Entities db = new Entities())
            {
                if (ID == 0)
                {
                    return "";
                }
                else
                {
                    return db.REF_LOCATION_TYPE_TB.Where(u => u.N_LOCATION_TYPE_SYSID == ID).Select(u => u.SZ_DESCRIPTION).FirstOrDefault().ToString();
                }
            }
        }
    }
}