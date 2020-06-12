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
    public class ResultTblGet
    {
        /// <summary>
        /// to get all results by sample id
        /// </summary>
        public IEnumerable<TRN_RESULT_TB> GetResultsBySampleID(int SampleID)
        {
            using (Entities db = new Entities())
            {
                var results = db.TRN_RESULT_TB
                                 .Where(u => u.N_SAMPLE_SYSID == SampleID)
                                 .OrderBy(u => u.N_RESULT_SYSID)
                                 .ToList();

                return (IEnumerable<TRN_RESULT_TB>)results;
            }
        }
    }
}