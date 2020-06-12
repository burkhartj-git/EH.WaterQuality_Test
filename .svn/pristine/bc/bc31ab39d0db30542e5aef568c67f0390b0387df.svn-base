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
    public class SampleLogGet
    {
        /// <summary>
        /// to get the sample log record by sample log id
        /// </summary>
        public TRN_SAMPLE_TB GetSampleLogByID(int SampleLogID)
        {
            using (Entities db = new Entities())
            {
                TRN_SAMPLE_TB sample = db.TRN_SAMPLE_TB
                                            .Where(u => u.N_SAMPLE_SYSID == SampleLogID)
                                            .FirstOrDefault();
                return sample;
            }
        }

        /// <summary>
        /// to get the sample log id next value
        /// </summary>
        public int GetNextSampleLogID()
        {
            using (Entities db = new Entities())
            {
                int sampleLogID = db.TRN_SAMPLE_TB.OrderByDescending(u => u.N_SAMPLE_SYSID)
                                    .Select(u => u.N_SAMPLE_SYSID)
                                    .FirstOrDefault();
                return sampleLogID + 1;
            }
        }

        /// <summary>
        /// to get all samples by location list and collected date
        /// </summary>
        public IEnumerable<TRN_SAMPLE_TB> GetSampleLogsByLocationsAndCollectedDate(List<int> LocationIDs, DateTime CollectedDate)
        {
            using (Entities db = new Entities())
            {
                var samples = db.TRN_SAMPLE_TB
                                .Where(u => u.DT_COLLECTED == CollectedDate
                                    && LocationIDs.Contains(u.N_LOCATION_SYSID))
                                .OrderBy(u => u.N_LOCATION_SYSID)
                                .ToList();

                return (IEnumerable<TRN_SAMPLE_TB>)samples;
            }
        }

        /// <summary>
        /// to get all samples by request group id and collected date
        /// </summary>
        public IEnumerable<TRN_SAMPLE_TB> GetSampleLogsByRequestGroupAndCollectedDate(int RequestGroupID, DateTime CollectedDate)
        {
            using (Entities db = new Entities())
            {
                var samples = db.TRN_SAMPLE_TB
                                .Where(u => u.DT_COLLECTED == CollectedDate
                                    && u.N_REQUEST_GROUP_SYSID == RequestGroupID)
                                .OrderBy(u => u.N_LOCATION_SYSID)
                                .ToList();
                return (IEnumerable<TRN_SAMPLE_TB>)samples;
            }
        }

        /// <summary>
        /// to get all samples by collected date where request group is null
        /// </summary>
        public IEnumerable<TRN_SAMPLE_TB> GetSampleLogsByCollectedDateWhenRequestGroupIsNull(DateTime CollectedDate)
        {
            using (Entities db = new Entities())
            {
                var samples = db.TRN_SAMPLE_TB
                                .Where(u => u.DT_COLLECTED == CollectedDate
                                    && u.N_REQUEST_GROUP_SYSID == null)
                                .OrderBy(u => u.N_LOCATION_SYSID)
                                .ToList();
                return (IEnumerable<TRN_SAMPLE_TB>)samples;
            }
        }

        /// <summary>
        /// to get all samples by sample log id list
        /// </summary>
        public IEnumerable<TRN_SAMPLE_TB> GetSampleLogsBySampleIDs()
        {
            using (Entities db = new Entities())
            {
                List<int> list = new List<int>();

                for (int i = 0; i < SampleList.SampleLogIDs.GetLength(0); i++)
                {
                    list.Add(SampleList.SampleLogIDs[i, 0]);
                }

                var samples = db.TRN_SAMPLE_TB
                                .Where(u => list.Contains(u.N_SAMPLE_SYSID))
                                .OrderBy(u => u.N_LOCATION_SYSID)
                                .ToList();

                return (IEnumerable<TRN_SAMPLE_TB>)samples;
            }
        }
    }
}