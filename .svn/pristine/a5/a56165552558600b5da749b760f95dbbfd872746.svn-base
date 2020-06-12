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
    public class SampleTblGet
    {
        /// <summary>
        /// to get all samples
        /// </summary>
        public IEnumerable<TRN_SAMPLE_TB> GetSamples()
        {
            using (Entities db = new Entities())
            {
                IEnumerable<TRN_SAMPLE_TB> samples = db.TRN_SAMPLE_TB
                                                        .OrderBy(u => u.N_SAMPLE_SYSID)
                                                        .ToList();
                return samples;
            }
        }
    }
}