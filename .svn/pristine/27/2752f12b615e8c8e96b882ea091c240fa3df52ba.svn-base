using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EHWaterQuality.Models;
using EHWaterQuality.Utilities;

namespace EHWaterQuality.Repositories
{
    public class WaterBodyGet
    {
        /// <summary>
        /// to get all water bodies
        /// </summary>
        public IEnumerable<SelectListItem> GetWaterBodies()
        {
            using (Entities db = new Entities())
            {
                IEnumerable<REF_WATER_BODY_TB> waterBodiesOrdered = (IEnumerable<REF_WATER_BODY_TB>)db.REF_WATER_BODY_TB
                                                .OrderBy(u => u.SZ_DESCRIPTION)
                                                .ToList();

                IEnumerable<SelectListItem> waterBodies = waterBodiesOrdered
                                .Select(u => new SelectListItem { Value = u.N_WATER_BODY_SYSID.ToString(), Text = u.SZ_DESCRIPTION })
                                .Distinct()
                                .ToList();

                return waterBodies;
            }
        }

        /// <summary>
        /// to check if the water body name and description combo exists
        /// </summary>
        public bool IsWaterBodyNameAndDescriptionDuplicate(string WaterBodyName, string WaterBodyDescription)
        {
            using (Entities db = new Entities())
            {
                return db.REF_WATER_BODY_TB.Any(r => r.SZ_NAME == WaterBodyName && r.SZ_DESCRIPTION == WaterBodyDescription);
            }
        }

        /// <summary>
        /// to get the water body description based on id
        /// </summary>
        public string GetWaterBodyByID(int ID)
        {
            using (Entities db = new Entities())
            {
                if (ID == 0)
                {
                    return "";
                }
                else
                {
                    return db.REF_WATER_BODY_TB.Where(u => u.N_WATER_BODY_SYSID == ID).Select(u => u.SZ_DESCRIPTION).FirstOrDefault().ToString();
                }
            }
        }
    }
}