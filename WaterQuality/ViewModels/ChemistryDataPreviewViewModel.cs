using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EHWaterQuality.Utilities;

namespace EHWaterQuality.ViewModels
{
    public class ChemistryDataPreviewViewModel
    {
        public List<ChemistryDataPreviewEditorViewModel> ChemistryDataPreviews {get; set;}
        public bool HasErrors { get; set; }
        public string ExcelFilePath { get; set; }
        public string ErrorMessage { get; set; }
        public int SelectedTestType { get; set; }

        public ChemistryDataPreviewViewModel()
        {
            this.ChemistryDataPreviews = new List<ChemistryDataPreviewEditorViewModel>();
        }
    }
}