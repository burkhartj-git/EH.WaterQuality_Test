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
    public class ResultTblController : BaseController
    {
        private GenericEHWaterQualityUnitOfWork _uow = null;
        private ResultTblGet _resultTblRepo = null;

        private string _modifiedBy = null;
        private string _userRole = null;
        //private WebServerClient _webServerClient;
        //private HttpClient _client;
        //private Uri _resourceServerUri = new Uri(ConfigurationManager.AppSettings["OAuth.MacombCounty.Paths.Remote.ResourceServerBaseAddress"]);
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ResultTblController()
        {
            _uow = new GenericEHWaterQualityUnitOfWork();
            _resultTblRepo = new ResultTblGet();
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
                    ViewBag.Message = "Function: ResultTblController.Index_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: ResultTblController.Index_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult GetResults(string id)
        {
            try
            {
                int sampleID = Convert.ToInt32(id);
                IEnumerable<TRN_RESULT_TB> results = _resultTblRepo.GetResultsBySampleID(sampleID);
                List<ResultTblIndexViewModel> resultList = new List<ResultTblIndexViewModel>();

                foreach (var result in results.OrderBy(u => u.N_RESULT_SYSID))
                {
                    string test = _uow.Repository<REF_TEST_TB>().Find(u => u.N_TEST_SYSID == result.N_TEST_SYSID).FirstOrDefault().SZ_DESCRIPTION;

                    ResultTblIndexViewModel item = new ResultTblIndexViewModel()
                    {
                        ResultID = result.N_RESULT_SYSID.ToString(),
                        ResultValue = result.N_RESULT_VALUE.ToString(),
                        ResultValueIndicator = result.SZ_RESULT_VALUE_INDICATOR,
                        SampleID = result.N_SAMPLE_SYSID.ToString(),
                        Test = test
                    };
                    resultList.Add(item);
                }

                return Json(resultList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: ResultTblController.GetResults_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: ResultTblController.GetResults_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult Delete(ResultTblDeleteViewModel model)
        {
            try
            {
                TRN_RESULT_TB result = _uow.Repository<TRN_RESULT_TB>().GetById(model.ID);
                ResultTblDeleteViewModel resultTblDeleteViewModel = new ResultTblDeleteViewModel()
                {
                    ID = result.N_RESULT_SYSID,
                    Message = "",
                    ShowMessage = false
                };

                return View("Delete", resultTblDeleteViewModel);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: ResultTblController.Delete_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: ResultTblController.Delete_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult DeleteResult(ResultTblDeleteViewModel model)
        {
            try
            {
                int id = Convert.ToInt32(model.ID);
                _uow.Repository<TRN_RESULT_TB>().Delete(id);
                _uow.SaveChanges();

                return RedirectToAction("Index", new { ID = id });
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: ResultTblController.Delete_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: ResultTblController.Delete_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }
    }
}