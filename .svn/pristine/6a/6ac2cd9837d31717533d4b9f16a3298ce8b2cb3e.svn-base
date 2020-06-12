using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EHWaterQuality.Models;

namespace EHWaterQuality.Repositories
{
    public class RequestGroupGet
    {
        /// <summary>
        /// to get all request groups
        /// </summary>
        public IEnumerable<SelectListItem> GetRequestGroups()
        {
            using (Entities db = new Entities())
            {
                IEnumerable<REF_REQUEST_GROUP_TB> requestGroupsOrdered = (IEnumerable<REF_REQUEST_GROUP_TB>)db.REF_REQUEST_GROUP_TB
                                                                            .OrderBy(u => u.SZ_DESCRIPTION)
                                                                            .ToList();
                IEnumerable<SelectListItem> requestGroups = requestGroupsOrdered
                                .Select(u => new SelectListItem { Value = u.N_REQUEST_GROUP_SYSID.ToString(), Text = u.SZ_DESCRIPTION })
                                .Distinct()
                                .ToList();
                return requestGroups;
            }
        }

        /// <summary>
        /// to check if the request group description exists
        /// </summary>
        public bool IsRequestGroupDescriptionDuplicate(string RequestGroupDescription)
        {
            using (Entities db = new Entities())
            {
                return db.REF_REQUEST_GROUP_TB.Any(r => r.SZ_DESCRIPTION == RequestGroupDescription);
            }
        }

        /// <summary>
        /// lookup - to get request group description by request group id
        /// </summary>
        public string GetRequestGroupDescriptionByRequestGroupID(int RequestGroupID)
        {
            using (Entities db = new Entities())
            {
                if (RequestGroupID == 0)
                {
                    return "";
                }
                else
                {
                    return db.REF_REQUEST_GROUP_TB.Where(u => u.N_REQUEST_GROUP_SYSID == RequestGroupID).Select(u => u.SZ_DESCRIPTION).FirstOrDefault().ToString();
                }
            }
        }
    }
}