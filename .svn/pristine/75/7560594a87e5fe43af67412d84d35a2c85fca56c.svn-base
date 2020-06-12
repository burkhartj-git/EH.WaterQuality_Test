using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EHWaterQuality.Models;
using EHWaterQuality.Utilities;

namespace EHWaterQuality.Repositories
{
    public class LocationGet
    {
        /// <summary>
        ///  to get location string by request group id
        /// </summary>
        public IEnumerable<Location> GetLocationsByRequestGroupID(int RequestGroupID)
        {
            using (Entities db = new Entities())
            {
                var locations = (from rg in db.REF_REQUEST_GROUP_TB
                                    join r in db.REF_REQUEST_TB
                                    on rg.N_REQUEST_GROUP_SYSID
                                    equals r.N_REQUEST_GROUP_SYSID
                                        join lg in db.REF_LOCATION_GROUP_TB
                                        on r.N_LOCATION_GROUP_SYSID
                                        equals lg.N_LOCATION_GROUP_SYSID
                                            join llg in db.REF_LOCATION_X_LOCATION_GROUP_TB
                                            on lg.N_LOCATION_GROUP_SYSID
                                            equals llg.N_LOCATION_GROUP_SYSID
                                                join l in db.REF_LOCATION_TB
                                                on llg.N_LOCATION_SYSID
                                                equals l.N_LOCATION_SYSID
                                    where rg.N_REQUEST_GROUP_SYSID == RequestGroupID
                                    orderby l.N_LOCATION_SYSID
                                    select new Location{ ID = l.N_LOCATION_SYSID.ToString(), Description = l.SZ_DESCRIPTION, RequestGroupDescription = rg.SZ_DESCRIPTION, RequestGroupID = rg.N_REQUEST_GROUP_SYSID.ToString(), Label = l.SZ_LABEL })
                                    .ToList();
                return (IEnumerable<Location>)locations;
            }
        }        

        /// <summary>
        /// lookup - to get location description by location id
        /// </summary>
        public string GetLocationDescriptionByLocationID(int LocationID)
        {
            using (Entities db = new Entities())
            {
                return db.REF_LOCATION_TB.Where(u => u.N_LOCATION_SYSID == LocationID).Select(u => u.SZ_DESCRIPTION).FirstOrDefault();                
            }
        }

        /// <summary>
        /// lookup - to get location label by location id
        /// </summary>
        public string GetLocationLabelByLocationID(int LocationID)
        {
            using (Entities db = new Entities())
            {
                return db.REF_LOCATION_TB.Where(u => u.N_LOCATION_SYSID == LocationID).Select(u => u.SZ_LABEL).FirstOrDefault();
            }
        }

        /// <summary>
        /// to get all location groups by location
        /// </summary>
        public IEnumerable<REF_LOCATION_X_LOCATION_GROUP_TB> GetLocationGroupsByLocationID(int LocationID)
        {
            using (Entities db = new Entities())
            {
                IEnumerable<REF_LOCATION_X_LOCATION_GROUP_TB> results = (IEnumerable<REF_LOCATION_X_LOCATION_GROUP_TB>)db.REF_LOCATION_X_LOCATION_GROUP_TB
                                    .Include("REF_LOCATION_GROUP_TB")
                                    .Where(u => u.N_LOCATION_SYSID == LocationID)
                                    .OrderBy(u => u.REF_LOCATION_GROUP_TB.SZ_DESCRIPTION)
                                    .ToList();

                return results;
            }
        }

        /// <summary>
        /// to get all locations by location group
        /// </summary>
        public IEnumerable<REF_LOCATION_X_LOCATION_GROUP_TB> GetLocationsByLocationGroupID(int LocationGroupID)
        {
            using (Entities db = new Entities())
            {
                IEnumerable<REF_LOCATION_X_LOCATION_GROUP_TB> results = (IEnumerable<REF_LOCATION_X_LOCATION_GROUP_TB>)db.REF_LOCATION_X_LOCATION_GROUP_TB
                                    .Include("REF_LOCATION_TB")
                                    .Where(u => u.N_LOCATION_GROUP_SYSID == LocationGroupID)
                                    .OrderBy(u => u.REF_LOCATION_TB.SZ_LABEL)
                                    .ThenBy(u => u.REF_LOCATION_TB.SZ_DESCRIPTION)
                                    .ToList();

                return results;
            }
        }

        /// <summary>
        /// to get select list for new location groups for location
        /// </summary>
        public IEnumerable<SelectListItem> GetLocationGroupsSelectListByLocationID(int LocationID)
        {
            using (Entities db = new Entities())
            {
                List<int> locationGroupIDs = (List<int>)db.REF_LOCATION_X_LOCATION_GROUP_TB
                                    .Where(u => u.N_LOCATION_SYSID == LocationID)
                                    .Select(u => u.N_LOCATION_GROUP_SYSID)
                                    .ToList();
                
                IEnumerable<REF_LOCATION_GROUP_TB> resultsOrdered = (IEnumerable<REF_LOCATION_GROUP_TB>)(from r in db.REF_LOCATION_GROUP_TB
                                                                                                             where !locationGroupIDs.Contains(r.N_LOCATION_GROUP_SYSID)
                                                                                                             orderby r.SZ_DESCRIPTION
                                                                                                             select r).ToList();

                IEnumerable<SelectListItem> results = resultsOrdered
                                    .Select(u => new SelectListItem { Value = u.N_LOCATION_GROUP_SYSID.ToString(), Text = u.SZ_DESCRIPTION })
                                    .Distinct()
                                    .ToList();

                return results;
            }
        }

        /// <summary>
        /// to get select list for new locations for location groups
        /// </summary>
        public IEnumerable<SelectListItem> GetLocationsSelectListByLocationGroupID(int LocationGroupID)
        {
            using (Entities db = new Entities())
            {
                List<int> locationIDs = (List<int>)db.REF_LOCATION_X_LOCATION_GROUP_TB
                                    .Where(u => u.N_LOCATION_GROUP_SYSID == LocationGroupID)
                                    .Select(u => u.N_LOCATION_SYSID)
                                    .ToList();

                IEnumerable<REF_LOCATION_TB> resultsOrdered = (IEnumerable<REF_LOCATION_TB>)(from r in db.REF_LOCATION_TB
                                                                                             where !locationIDs.Contains(r.N_LOCATION_SYSID)
                                                                                             orderby r.SZ_DESCRIPTION
                                                                                             select r).ToList();

                IEnumerable<SelectListItem> results = resultsOrdered
                                    .Select(u => new SelectListItem { Value = u.N_LOCATION_SYSID.ToString(), Text = u.SZ_LABEL + " - " + u.SZ_DESCRIPTION })
                                    .Distinct()
                                    .ToList();

                return results;
            }
        }

        /// <summary>
        /// to get select list for all location groups
        /// </summary>
        public IEnumerable<SelectListItem> GetLocationGroupsSelectList()
        {
            using (Entities db = new Entities())
            {
                IEnumerable<SelectListItem> resultsOrdered = (IEnumerable<SelectListItem>)db.REF_LOCATION_GROUP_TB
                                    .OrderBy(u => u.SZ_DESCRIPTION)
                                    .ToList()
                                    .Select(u => new SelectListItem { Value = u.N_LOCATION_GROUP_SYSID.ToString(), Text = u.SZ_DESCRIPTION })
                                    .Distinct()
                                    .ToList();

                return resultsOrdered;
            }
        }

        /// <summary>
        /// to delete location x location group by id
        /// </summary>
        public bool DeleteLocByLocGroupByIDs(int LocationID, int LocationGroupID)
        {
            using (Entities db = new Entities())
            {
                bool isDeleted = false;

                REF_LOCATION_X_LOCATION_GROUP_TB result = db.REF_LOCATION_X_LOCATION_GROUP_TB
                                        .FirstOrDefault(u => u.N_LOCATION_SYSID == LocationID && u.N_LOCATION_GROUP_SYSID == LocationGroupID);

                db.REF_LOCATION_X_LOCATION_GROUP_TB.Remove(result);
                db.SaveChanges();
                isDeleted = true;

                return isDeleted;                
            }
        }

        /// <summary>
        /// to check if the location group description exists
        /// </summary>
        public bool IsLocationGroupDescriptionDuplicate(string LocationGroupDescription)
        {
            using (Entities db = new Entities())
            {
                return db.REF_LOCATION_GROUP_TB.Any(r => r.SZ_DESCRIPTION == LocationGroupDescription);
            }
        }

        /// <summary>
        /// to get the location id by the location label
        /// </summary>
        public int GetLocationIDByLocationLabel(string Label)
        {
            using (Entities db = new Entities())
            {
                return db.REF_LOCATION_TB.Where(u => u.SZ_LABEL == Label).Select(u => u.N_LOCATION_SYSID).FirstOrDefault();
            }
        }

        /// <summary>
        /// to get the location group description by id
        /// </summary>
        public string GetLocationGroupDescriptionByID(int ID)
        {
            using (Entities db = new Entities())
            {
                if (ID == 0)
                {
                    return "";
                }
                else
                {
                    return db.REF_LOCATION_GROUP_TB.Where(u => u.N_LOCATION_GROUP_SYSID == ID).Select(u => u.SZ_DESCRIPTION).FirstOrDefault().ToString();
                }
            }
        }
    }
}