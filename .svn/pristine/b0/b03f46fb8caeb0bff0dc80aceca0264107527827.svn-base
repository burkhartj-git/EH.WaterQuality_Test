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
    public class SewerOverflowLogGet
    {
        /// <summary>
        /// to get all sewer overflow activity logs by facility, start date, and end date
        /// </summary>
        public IEnumerable<TRN_SEWER_OVERFLOW_TB> GetSewerOverflowActivityLogsByFacilityAndDates(int Facility, string StartDate, string EndDate)
        {
            Entities db = new Entities();
            var activities = (IEnumerable<TRN_SEWER_OVERFLOW_TB>)db.TRN_SEWER_OVERFLOW_TB
                                    .Include("REF_FACILITY_TB")
                                    .OrderBy(u => u.REF_FACILITY_TB.SZ_TITLE)
                                    .ThenBy(u => u.DT_ACTIVITY_START)
                                    .ThenBy(u => u.SZ_ACTIVITY_START_TIME)
                                    .ToList();
            if (Facility != 0)
            {
                activities = activities.Where(u => u.N_FACILITY_SYSID == Facility).ToList();
            }
            if (StartDate != "0")
            {
                DateTime dtStart;
                DateTime? startDate = DateTime.TryParse(StartDate, out dtStart) ? dtStart : (DateTime?)null;
                if (startDate != null)
                {
                    activities = activities.Where(u => u.DT_ACTIVITY_START >= startDate).ToList();
                }
            }
            if (EndDate != "0")
            {
                DateTime dtEnd;
                DateTime? endDate = DateTime.TryParse(EndDate, out dtEnd) ? dtEnd : (DateTime?)null;
                if (endDate != null)
                {
                    activities = activities.Where(u => u.DT_ACTIVITY_END <= endDate || u.DT_ACTIVITY_END == null).ToList();
                }
            }

            return activities;
        }
    }
}