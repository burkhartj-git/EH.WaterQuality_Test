using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using EHWaterQuality.Models;
using EHWaterQuality.Repositories;

namespace EHWaterQuality.Utilities
{
    public static class ChemistryExcelLinq
    {
        public static bool ExcelDataHasErrors { get; set; }

        public static List<ChemistryData> GetExcelChemistryData(string Filename)
        {
            var rows = ChemistryExcelLinq.GetExcelRange(Filename).ToList();
            List<ChemistryData> chemistryDatas = new List<ChemistryData>();
            ChemistryExcelLinq.ExcelDataHasErrors = false;

            if (rows.Count() > 0)
            {
                foreach (var row in rows)
                {
                    string siteID = (row.ElementAt(0) ?? "").ToString();
                    if (!siteID.ToUpper().Contains("D"))
                    {
                        string collectedDate = (row.ElementAt(1) ?? "").ToString();
                        string collectedTime = (row.ElementAt(2) ?? "").ToString();
                        string testName = (row.ElementAt(3) ?? "").ToString();
                        string sampleType = (row.ElementAt(4) ?? "").ToString();
                        string sampleMedia = (row.ElementAt(5) ?? "").ToString();
                        string reportedResult = (row.ElementAt(6) ?? "").ToString();
                        string units = (row.ElementAt(7) ?? "").ToString();
                        string flag = (row.ElementAt(8) ?? "").ToString();
                        string aMethod = (row.ElementAt(9) ?? "").ToString();

                        ChemistryData chemistryData = new ChemistryData()
                        {
                            SiteID = siteID,
                            CollectedDate = IsDate(collectedDate) ? collectedDate : "Invalid value: '" + collectedDate + "' should be a valid date.",
                            CollectedTime = collectedTime,
                            TestName = testName,
                            SampleType = sampleType,
                            SampleMedia = sampleMedia,
                            ReportedResult = IsNumeric(reportedResult) ? reportedResult : "Invalid value",
                            Units = units,
                            Flag = flag,
                            AMethod = aMethod
                        };

                        if (chemistryData.CollectedDate == "Invalid value" ||
                            chemistryData.ReportedResult == "Invalid value")
                        {
                            ChemistryExcelLinq.ExcelDataHasErrors = true;
                        }

                        chemistryDatas.Add(chemistryData);
                    }
                }

                return chemistryDatas.ToList();
            }
            else
            {
                return null;
            }
        }

        public static IEnumerable<List<dynamic>> GetExcelRange(string Filename)
        {
            var file = new FileInfo(Filename);
            using (var package = new ExcelPackage(file))
            {
                var ws = package.Workbook.Worksheets["Sheet1"];

                foreach (var worksheet in package.Workbook.Worksheets)
                {
                    if (worksheet.Cells[1, 1].Value.ToString() == "SITE ID")
                    {
                        ws = worksheet;
                    }
                }                

                if (ws.Cells[1, 1].Value == null ||
                    ws.Cells[1, 2].Value == null ||
                    ws.Cells[1, 3].Value == null ||
                    ws.Cells[1, 4].Value == null ||
                    ws.Cells[1, 5].Value == null ||
                    ws.Cells[1, 6].Value == null ||
                    ws.Cells[1, 7].Value == null ||
                    ws.Cells[1, 8].Value == null ||
                    ws.Cells[1, 9].Value == null ||
                    ws.Cells[1, 10].Value == null)
                {
                    ExcelFileInfo.ExcelFileError = "Please make sure that the Chemistry Data is correct and in the proper Excel format. The worksheet does not contain the proper header row titles";
                    yield break;
                }

                string siteIDCheck = ws.Cells[1, 1].Value.ToString();
                string collectDateCheck = ws.Cells[1, 2].Value.ToString();
                string collectTimeCheck = ws.Cells[1, 3].Value.ToString();
                string testNameCheck = ws.Cells[1, 4].Value.ToString();
                string sampleTypeCheck = ws.Cells[1, 5].Value.ToString();
                string sampleMediaCheck = ws.Cells[1, 6].Value.ToString();
                string reportedResultCheck = ws.Cells[1, 7].Value.ToString();
                string unitsCheck = ws.Cells[1, 8].Value.ToString();
                string flagCheck = ws.Cells[1, 9].Value.ToString();
                string amethodCheck = ws.Cells[1, 10].Value.ToString();

                if (siteIDCheck.ToUpper() != "SITE ID" ||
                        collectDateCheck.ToUpper() != "COLLECT DATE" ||
                        collectTimeCheck.ToUpper() != "COLLECT TIME" ||
                        testNameCheck.ToUpper() != "TEST NAME" ||
                        sampleTypeCheck.ToUpper() != "SAMPLE TYPE" ||
                        sampleMediaCheck.ToUpper() != "SAMPLE MEDIA" ||
                        reportedResultCheck.ToUpper() != "REPORTED RESULT" ||
                        unitsCheck.ToUpper() != "UNITS" ||
                        flagCheck.ToUpper() != "FLAG" ||
                        amethodCheck.ToUpper() != "AMETHOD")
                {
                    ExcelFileInfo.ExcelFileError = "Please make sure that the Chemistry Data is correct and in the proper Excel format.";
                    yield break;
                }

                for (int rowIndex = 2; rowIndex <= ws.Dimension.End.Row; rowIndex++)
                {
                    List<dynamic> myRow = new List<dynamic>(ws.Dimension.End.Row);

                    for (int colIndex = 1; colIndex <= ws.Dimension.End.Column; colIndex++)
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

        public static Boolean IsDate(string Date)
        {
            bool isDate = false;
            DateTime dateValue;

            if (DateTime.TryParse(Date, out dateValue)) isDate = true;

            return isDate;
        }
    }
}