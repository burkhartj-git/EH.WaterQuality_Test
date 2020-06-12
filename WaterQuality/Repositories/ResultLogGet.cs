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
    public class ResultLogGet
    {
        /// <summary>
        /// to get the result log id next value
        /// </summary>
        public int GetNextResultLogID()
        {
            using (Entities db = new Entities())
            {
                int resultLogID = db.TRN_RESULT_TB.OrderByDescending(u => u.N_RESULT_SYSID)
                                    .Select(u => u.N_RESULT_SYSID)
                                    .FirstOrDefault();
                return resultLogID + 1;
            }
        }

        /// <summary>
        /// to get all results by sample log id list
        /// </summary>
        public IEnumerable<TRN_RESULT_TB> GetResultLogsBySampleIDs()
        {
            using (Entities db = new Entities())
            {
                List<int> list = new List<int>();

                for (int i = 0; i < SampleList.SampleLogIDs.GetLength(0); i++)
                {
                    list.Add(SampleList.SampleLogIDs[i, 0]);
                }

                var results = db.TRN_RESULT_TB
                                .Include("TRN_SAMPLE_TB")
                                .Where(u => list.Contains(u.N_SAMPLE_SYSID))
                                .OrderBy(u => u.TRN_SAMPLE_TB.N_LOCATION_SYSID)
                                .ToList();

                return (IEnumerable<TRN_RESULT_TB>)results;                                
            }
        }
    }
}