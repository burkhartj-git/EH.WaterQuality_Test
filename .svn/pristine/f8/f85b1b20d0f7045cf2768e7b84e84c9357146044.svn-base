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
    public class WeatherDataLogGet
    {
        /// <summary>
        /// to get all weather data logs by month and year
        /// </summary>
        public IEnumerable<WeatherData> GetWeatherDataLogsByMonthAndYear(string Month, string Year)
        {
            using (Entities db = new Entities())
            {
                int month = GetNumericMonth(Month);
                int year = Convert.ToInt32(Year);
                List<WeatherData> weatherDatas = new List<WeatherData>();

                var samples = db.TRN_SAMPLE_TB
                                .Where(u => u.SZ_COLLECTED_BY == "Selfridge"
                                    && u.DT_COLLECTED.Month == month
                                    && u.DT_COLLECTED.Year == year)
                                .OrderBy(u => u.DT_COLLECTED)
                                .ToList();

                foreach (var sample in samples)
                {
                    string precip = db.TRN_RESULT_TB.Where(u => u.N_SAMPLE_SYSID == sample.N_SAMPLE_SYSID && u.N_TEST_SYSID == 15).Select(u => u.N_RESULT_VALUE).FirstOrDefault().ToString();

                    WeatherData weatherData = new WeatherData()
                    {
                        DateCollected = sample.DT_COLLECTED.ToShortDateString(),
                        TempMax = db.TRN_RESULT_TB.Where(u => u.N_SAMPLE_SYSID == sample.N_SAMPLE_SYSID && u.N_TEST_SYSID == 12).Select(u => u.N_RESULT_VALUE).FirstOrDefault().ToString(),
                        TempMin = db.TRN_RESULT_TB.Where(u => u.N_SAMPLE_SYSID == sample.N_SAMPLE_SYSID && u.N_TEST_SYSID == 13).Select(u => u.N_RESULT_VALUE).FirstOrDefault().ToString(),
                        TempMean = db.TRN_RESULT_TB.Where(u => u.N_SAMPLE_SYSID == sample.N_SAMPLE_SYSID && u.N_TEST_SYSID == 14).Select(u => u.N_RESULT_VALUE).FirstOrDefault().ToString(),
                        Precipitation24HrWaterEquiv = precip == "0.000010" ? "T" : precip,
                        MaxWindSpeed = db.TRN_RESULT_TB.Where(u => u.N_SAMPLE_SYSID == sample.N_SAMPLE_SYSID && u.N_TEST_SYSID == 16).Select(u => u.N_RESULT_VALUE).FirstOrDefault().ToString(),
                        MaxWindDirection = db.TRN_RESULT_TB.Where(u => u.N_SAMPLE_SYSID == sample.N_SAMPLE_SYSID && u.N_TEST_SYSID == 17).Select(u => u.N_RESULT_VALUE).FirstOrDefault().ToString(),
                        SampleID = sample.N_SAMPLE_SYSID.ToString()
                    };

                    weatherDatas.Add(weatherData);
                }

                return (IEnumerable<WeatherData>)weatherDatas;
            }
        }

        /// <summary>
        /// to get weather data log by string id
        /// </summary>
        public TBL_WEATHER_DATA GetWeatherDataLogByID(string WeatherDataLogID)
        {
            using (Entities db = new Entities())
            {
                return db.TBL_WEATHER_DATA
                            .Where(u => u.SZ_READING_DATE == WeatherDataLogID)
                            .FirstOrDefault();
            }
        }

        /// <summary>
        /// to get sample log by location id and collected date
        /// </summary>
        public TRN_SAMPLE_TB GetSampleLogByLocationIDAndCollectedDate(int LocationID, DateTime CollectedDate)
        {
            using (Entities db = new Entities())
            {
                return db.TRN_SAMPLE_TB
                            .Where(u => u.N_LOCATION_SYSID == LocationID && DbFunctions.TruncateTime(u.DT_COLLECTED) == DbFunctions.TruncateTime(CollectedDate))
                            .FirstOrDefault();
            }
        }

        /// <summary>
        /// to get location id for selridge ang base
        /// </summary>
        public int GetSelfridgeLocationID()
        {
            //find location for selfridge ang base from location table. if exists - use it, else - create new record
            using (Entities db = new Entities())
            {
                int selfridgeLocationID = db.REF_LOCATION_TB.Where(u => u.SZ_DESCRIPTION.ToUpper().Contains("SELFRIDGE")).Select(u => u.N_LOCATION_SYSID).FirstOrDefault();

                if (selfridgeLocationID == 0)
                {
                    REF_LOCATION_TB location = new REF_LOCATION_TB()
                    {
                        B_INACTIVE = false,
                        DT_MODIFIED = DateTime.UtcNow,
                        DT_ENTERED = DateTime.UtcNow,
                        SZ_DESCRIPTION = "Selfridge",
                        SZ_ENTERED_BY = SessionHelper.UserName,
                        SZ_LABEL = "-1",
                        SZ_MODIFIED_BY = SessionHelper.UserName
                    };
                    db.REF_LOCATION_TB.Add(location);
                    db.SaveChanges();

                    return db.REF_LOCATION_TB.Where(u => u.SZ_DESCRIPTION.ToUpper().Contains("SELFRIDGE")).Select(u => u.N_LOCATION_SYSID).FirstOrDefault();
                }
                else
                {
                    return selfridgeLocationID;
                }
            }
        }

        private int GetNumericMonth(string Month)
        {
            switch (Month)
            {
                case "JAN":
                    return 1;
                case "FEB":
                    return 2;
                case "MAR":
                    return 3;
                case "APR":
                    return 4;
                case "MAY":
                    return 5;
                case "JUN":
                    return 6;
                case "JUL":
                    return 7;
                case "AUG":
                    return 8;
                case "SEP":
                    return 9;
                case "OCT":
                    return 10;
                case "NOV":
                    return 11;
                case "DEC":
                    return 12;
                default:
                    return 0;
            }
        }
    }
}