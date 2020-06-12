using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Identity;
using DotNetOpenAuth.OAuth2;
using EHWaterQuality.Filters;
using EHWaterQuality.Models;
using EHWaterQuality.Repositories;
using EHWaterQuality.Utilities;
using EHWaterQuality.ViewModels;

namespace EHWaterQuality.Controllers
{
    [MacAuthorize(Action = "AdministerAll,AdministerOwn")]
    public class SampleTblController : BaseController
    {
        private GenericEHWaterQualityUnitOfWork _uow = null;
        private SampleTblGet _sampleTblRepo = null;

        private string _modifiedBy = null;
        private string _userRole = null;
        //private WebServerClient _webServerClient;
        //private HttpClient _client;
        //private Uri _resourceServerUri = new Uri(ConfigurationManager.AppSettings["OAuth.MacombCounty.Paths.Remote.ResourceServerBaseAddress"]);
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public SampleTblController()
        {
            _uow = new GenericEHWaterQualityUnitOfWork();
            _sampleTblRepo = new SampleTblGet();
            if (SessionHelper.UserName == null)
            {
                _modifiedBy = "Test";
            }
            else
            {
                _modifiedBy = SessionHelper.UserName;
            }
        }

        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: SampleTblController.Index_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: SampleTblController.Index_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult GetSamples()
        {
            try
            {
                IEnumerable<TRN_SAMPLE_TB> samples = _uow.Repository<TRN_SAMPLE_TB>().GetAll();
                List<SampleTblIndexViewModel> sampleList = new List<SampleTblIndexViewModel>();

                foreach (var sample in samples.OrderBy(u => u.N_SAMPLE_SYSID))
                {
                    string location = _uow.Repository<REF_LOCATION_TB>().Find(u => u.N_LOCATION_SYSID == sample.N_LOCATION_SYSID).FirstOrDefault().SZ_LABEL;

                    SampleTblIndexViewModel item = new SampleTblIndexViewModel()
                    {
                        CollectedBy = sample.SZ_COLLECTED_BY,
                        CollectedTime = sample.N_COLLECTED_TIME,
                        DateCollected = sample.DT_COLLECTED.ToShortDateString(),
                        Location = location,
                        SampleID = sample.N_SAMPLE_SYSID.ToString()
                    };
                    sampleList.Add(item);
                }

                return Json(sampleList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: SampleTblController.GetSamples_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: SampleTblController.GetSamples_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult Delete(SampleTblDeleteViewModel model)
        {
            try
            {
                TRN_SAMPLE_TB sample = _uow.Repository<TRN_SAMPLE_TB>().GetById(model.ID);
                SampleTblDeleteViewModel sampleTblDeleteViewModel = new SampleTblDeleteViewModel()
                {
                    ID = sample.N_SAMPLE_SYSID,
                    Message = "",
                    ShowMessage = false
                };

                return View("Delete", sampleTblDeleteViewModel);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: SampleTblController.Delete_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: SampleTblController.Delete_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult DeleteSample(SampleTblDeleteViewModel model)
        {
            try
            {
                int id = Convert.ToInt32(model.ID);
                if (_uow.Repository<TRN_RESULT_TB>().Find(u => u.N_SAMPLE_SYSID == id).Count() > 0)
                {
                    model.ShowMessage = true;
                    model.Message = "Result records exist for this Sample. Sorry, the Sample can not be deleted.";

                    return View("Delete", model);
                }
                else
                {
                    _uow.Repository<TRN_SAMPLE_TB>().Delete(id);
                    _uow.SaveChanges();

                    return RedirectToAction("Index", new { ID = id });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: SampleTblController.Delete_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: SampleTblController.Delete_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }
    }
}