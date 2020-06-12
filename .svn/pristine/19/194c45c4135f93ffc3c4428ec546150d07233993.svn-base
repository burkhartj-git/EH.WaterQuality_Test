using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using EHWaterQuality.Models;
using EHWaterQuality.Repositories;

namespace EHWaterQuality.Utilities
{
    public static class WeatherExcelLinq
    {
        public static bool ExcelDataHasErrors { get; set; }
        public static HashSet<string> Months { get; set; }
        public static GenericEHWaterQualityUnitOfWork _uow = null;

        static WeatherExcelLinq()
        {
            Months = new HashSet<string>() { "JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC" };
            ExcelFileInfo.ExcelFileError = "";
            _uow = new GenericEHWaterQualityUnitOfWork();
        }

        public static List<WeatherData> GetExcelWeatherData(string Filename)
        {
            var rows = WeatherExcelLinq.GetExcelRange(Filename).ToList();
            List<WeatherData> weatherDatas = new List<WeatherData>();
            int refCol = 11;
            int calCol = 0;
            string monthRef = "Snow";
            string yearRef = "Depth";
            WeatherExcelLinq.ExcelDataHasErrors = false;

            if (rows.Count() > 0)
            {
                string month = rows[0].ElementAt(0).ToString();
                string year = rows[1].ElementAt(0).ToString();

                foreach (var row in rows)
                {
                    string reference = (row.ElementAt(refCol) ?? "").ToString();
                    string calendar = (row.ElementAt(calCol) ?? "").ToString();

                    if (reference != monthRef && reference != yearRef && calendar != "Day")
                    {
                        string day = (row.ElementAt(calCol) ?? "").ToString();
                        string maxWindDirection = (row.ElementAt(9) ?? "").ToString();
                        string maxWindSpeed = (row.ElementAt(8) ?? "").ToString();
                        string precip24HrSnow = (row.ElementAt(7) ?? "").ToString();
                        string precip24HrWater = (row.ElementAt(6) ?? "").ToString();
                        string snowDepth = (row.ElementAt(11) ?? "").ToString();
                        string tempDays65Cooling = (row.ElementAt(5) ?? "").ToString();
                        string tempDegBaseHeating = (row.ElementAt(4) ?? "").ToString();
                        string tempMax = (row.ElementAt(1) ?? "").ToString();
                        string tempMean = (row.ElementAt(3) ?? "").ToString();
                        string tempMin = (row.ElementAt(2) ?? "").ToString();
                        string tstmDays = (row.ElementAt(10) ?? "").ToString();

                        WeatherData weatherData = new WeatherData()
                        {
                            Day = IsNumeric(day) && IsDayValid(day) ? day : "Invalid value: '" + day + "' should be a number between 1 and 31.",
                            TempMax = IsNumeric(tempMax) || tempMax.ToUpper() == "N" ? tempMax : "Invalid value",
                            TempMin = IsNumeric(tempMin) || tempMin.ToUpper() == "N" ? tempMin : "Invalid value",
                            TempMean = IsNumeric(tempMean) || tempMean.ToUpper() == "N" ? tempMean : "Invalid value",
                            Precipitation24HrWaterEquiv = IsNumeric(precip24HrWater) || IsWaterPrecipValid(precip24HrWater) || precip24HrWater.ToUpper() == "N" ? precip24HrWater : "Invalid value",
                            MaxWindDirection = IsNumeric(maxWindDirection) || maxWindDirection.ToUpper() == "N" ? maxWindDirection : "Invalid value",
                            MaxWindSpeed = IsNumeric(maxWindSpeed) || maxWindSpeed.ToUpper() == "N" ? maxWindSpeed : "Invalid value",
                            Month = month,
                            Year = year
                        };

                        if (weatherData.Day == "Invalid value" ||
                            weatherData.MaxWindDirection == "Invalid value" ||
                            weatherData.MaxWindSpeed == "Invalid value" ||
                            weatherData.Precipitation24HrWaterEquiv == "Invalid value" ||
                            weatherData.TempMax == "Invalid value" ||
                            weatherData.TempMean == "Invalid value" ||
                            weatherData.TempMin == "Invalid value")
                        {
                            WeatherExcelLinq.ExcelDataHasErrors = true;
                        }

                        weatherDatas.Add(weatherData);
                    }
                }

                return weatherDatas.OrderBy(u => u.Day.Length).ThenBy(u => u.Day).ToList();
            }
            else
            {
                return null;
            }
        }

        public static IEnumerable<List<dynamic>> GetExcelRange(string FileName)
        {
            var file = new FileInfo(FileName);
            using (var package = new ExcelPackage(file))
            {
                var ws = package.Workbook.Worksheets["Sheet1"];

                if (ws.Cells[4, 1].Value == null)
                {
                    ExcelFileInfo.ExcelFileError = "Please make sure that the Weather Data is correct and in the proper Excel format. There is no value in Cells[Row 4, Column 1].";
                    yield break;
                }
                string dayCheck = ws.Cells[4, 1].Value.ToString();
                if (dayCheck.ToUpper() != "DAY")
                {
                    ExcelFileInfo.ExcelFileError = "Please make sure that the Weather Data is correct and in the proper Excel format. The value in Cells[Row 4, Column 1] should be 'DAY'.";
                    yield break;
                }

                if (ws.Cells[2, 1].Value == null)
                {
                    ExcelFileInfo.ExcelFileError = "Please make sure that the Weather Data is correct and in the proper Excel format. There is no value in Cells[Row 2, Column 1].";
                    yield break;
                }
                string month = ws.Cells[2, 1].Value.ToString().ToUpper();
                if (IsMonthValid(month) != true)
                {
                    ExcelFileInfo.ExcelFileError = "Invalid month: '" + month + "' specified in Weather Data file.";
                    yield break;
                }


                if (ws.Cells[3, 1].Value == null)
                {
                    ExcelFileInfo.ExcelFileError = "Please make sure that the Weather Data is correct and in the proper Excel format. There is no value in Cells[Row 3, Column 1].";
                    yield break;
                }
                string year = ws.Cells[3, 1].Value.ToString();
                if (IsYearValid(year) != true)
                {
                    ExcelFileInfo.ExcelFileError = "Invalid year: '" + year + "' specified in Weather Data file.";
                    yield break;
                }

                //check that data has correct number of records for month
                if (ws.Cells[2, 1].Value.ToString() == "JAN" && ws.Cells[36, 1].Value.ToString() != "Total")
                {
                    ExcelFileInfo.ExcelFileError = "Please make sure that the Weather Data is correct. January has 31 days.";
                    yield break;
                }
                if (ws.Cells[2, 1].Value.ToString() == "FEB" && IsLeapYear(ws.Cells[3, 1].Value.ToString()) && ws.Cells[34, 1].Value.ToString() != "Total")
                {
                    ExcelFileInfo.ExcelFileError = "Please make sure that the Weather Data is correct. February has 29 days for leap years.";
                    yield break;
                }
                if (ws.Cells[2, 1].Value.ToString() == "FEB" && !IsLeapYear(ws.Cells[3, 1].Value.ToString()) && ws.Cells[33, 1].Value.ToString() != "Total")
                {
                    ExcelFileInfo.ExcelFileError = "Please make sure that the Weather Data is correct. February has 28 days.";
                    yield break;
                }
                if (ws.Cells[2, 1].Value.ToString() == "MAR" && ws.Cells[36, 1].Value.ToString() != "Total")
                {
                    ExcelFileInfo.ExcelFileError = "Please make sure that the Weather Data is correct. March has 31 days.";
                    yield break;
                }
                if (ws.Cells[2, 1].Value.ToString() == "APR" && ws.Cells[35, 1].Value.ToString() != "Total")
                {
                    ExcelFileInfo.ExcelFileError = "Please make sure that the Weather Data is correct. April has 30 days.";
                    yield break;
                }
                if (ws.Cells[2, 1].Value.ToString() == "MAY" && ws.Cells[36, 1].Value.ToString() != "Total")
                {
                    ExcelFileInfo.ExcelFileError = "Please make sure that the Weather Data is correct. May has 31 days.";
                    yield break;
                }
                if (ws.Cells[2, 1].Value.ToString() == "JUN" && ws.Cells[35, 1].Value.ToString() != "Total")
                {
                    ExcelFileInfo.ExcelFileError = "Please make sure that the Weather Data is correct. June has 30 days.";
                    yield break;
                }
                if (ws.Cells[2, 1].Value.ToString() == "JUL" && ws.Cells[36, 1].Value.ToString() != "Total")
                {
                    ExcelFileInfo.ExcelFileError = "Please make sure that the Weather Data is correct. July has 31 days.";
                    yield break;
                }
                if (ws.Cells[2, 1].Value.ToString() == "AUG" && ws.Cells[36, 1].Value.ToString() != "Total")
                {
                    ExcelFileInfo.ExcelFileError = "Please make sure that the Weather Data is correct. August has 31 days.";
                    yield break;
                }
                if (ws.Cells[2, 1].Value.ToString() == "SEP" && ws.Cells[35, 1].Value.ToString() != "Total")
                {
                    ExcelFileInfo.ExcelFileError = "Please make sure that the Weather Data is correct. September has 30 days.";
                    yield break;
                }
                if (ws.Cells[2, 1].Value.ToString() == "OCT" && ws.Cells[36, 1].Value.ToString() != "Total")
                {
                    ExcelFileInfo.ExcelFileError = "Please make sure that the Weather Data is correct. October has 31 days.";
                    yield break;
                }
                if (ws.Cells[2, 1].Value.ToString() == "NOV" && ws.Cells[35, 1].Value.ToString() != "Total")
                {
                    ExcelFileInfo.ExcelFileError = "Please make sure that the Weather Data is correct. November has 30 days.";
                    yield break;
                }
                if (ws.Cells[2, 1].Value.ToString() == "DEC" && ws.Cells[36, 1].Value.ToString() != "Total")
                {
                    ExcelFileInfo.ExcelFileError = "Please make sure that the Weather Data is correct. December has 31 days.";
                    yield break;
                }

                //check for duplicate month / year table data
                IEnumerable<TBL_WEATHER_DATA> weatherDatas =
                    (IEnumerable<TBL_WEATHER_DATA>)_uow.Repository<TBL_WEATHER_DATA>()
                                    .Find(u => u.SZ_READING_DATE.Contains(month) && u.SZ_READING_DATE.Contains(year));
                if (weatherDatas.Count() > 0)
                {
                    ExcelFileInfo.ExcelFileError = "Weather Data already exists for: '" + month + ", " + year + "'.";
                    yield break;
                }

                int rowCount = 0;
                bool isLeapYear = IsLeapYear(year);

                switch (month)
                {
                    case "JAN":
                    case "MAR":
                    case "MAY":
                    case "JUL":
                    case "AUG":
                    case "OCT":
                    case "DEC":
                        rowCount = 35;
                        break;
                    case "APR":
                    case "JUN":
                    case "SEP":
                    case "NOV":
                        rowCount = 34;
                        break;
                    case "FEB":
                        if (isLeapYear)
                        {
                            rowCount = 33;
                        }
                        else
                        {
                            rowCount = 32;
                        }
                        break;
                    default:
                        break;
                }

                for (int rowIndex = 2; rowIndex <= rowCount; rowIndex++)
                {
                    List<dynamic> myRow = new List<dynamic>(rowCount);

                    for (int colIndex = 1; colIndex <= 12; colIndex++)
                    {
                        string text = "";
                        if (ws.Cells[rowIndex, colIndex].Value != null)
                        {
                            myRow.Add(ws.Cells[rowIndex, colIndex].Value.ToString());
                        }
                        else
                        {
                            myRow.Add(text);
                        }
                    }

                    yield return myRow;
                }
            }
        }

        public static Boolean IsNumeric(string Number)
        {
            Double result;
            bool isNumeric = Double.TryParse(Number, out result);

            return isNumeric;
        }

        public static Boolean IsDayValid(string Number)
        {
            int result;
            try
            {
                Int32.TryParse(Number, out result);
                if (result < 0 || result > 31)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static Boolean IsMonthValid(string Month)
        {
            bool isValid = false;
            if (Months.Contains(Month.ToUpper())) isValid = true;

            return isValid;
        }

        public static Boolean IsYearValid(string Year)
        {
            bool isValid = true;
            int currentYear = Convert.ToInt32(DateTime.UtcNow.Year);
            int refYear = Convert.ToInt32(Year);

            if (refYear > currentYear || refYear < 1950) isValid = false;

            return isValid;
        }

        public static Boolean IsWaterPrecipValid(string Precip)
        {
            bool isValid = false;

            //"T" = trace
            if (Precip == "T") isValid = true;

            return isValid;
        }

        public static Boolean IsLeapYear(string Year)
        {
            bool isLeapYear = false;
            int year = Convert.ToInt32(Year);

            if (year % 4 == 0 && year % 100 != 0 || year % 400 == 0) isLeapYear = true;

            return isLeapYear;
        }
    }
}