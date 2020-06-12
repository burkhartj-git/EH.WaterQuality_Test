using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
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
    public class WaterShedController : BaseController
    {
        private GenericEHWaterQualityUnitOfWork _uow = null;
        private WaterShedGet _waterShedRepo = null;

        private string _modifiedBy = null;
        private string _userRole = null;
        //private WebServerClient _webServerClient;
        //private HttpClient _client;
        //private Uri _resourceServerUri = new Uri(ConfigurationManager.AppSettings["OAuth.MacombCounty.Paths.Remote.ResourceServerBaseAddress"]);

        public WaterShedController()
        {
            _uow = new GenericEHWaterQualityUnitOfWork();
            _waterShedRepo = new WaterShedGet();

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
        public ViewResult Index(int ID)
        {
            var model = new WaterShedIndexViewModel()
            {
                ID = ID
            };

            return View(model);
        }

        [HttpGet]
        public JsonResult GetWaterSheds()
        {
            IEnumerable<REF_LOCATION_TYPE_TB> waterSheds = _uow.Repository<REF_LOCATION_TYPE_TB>().GetAll();
            List<WaterShedIndex> waterShedList = new List<WaterShedIndex>();

            waterSheds = waterSheds.ToList().OrderBy(u => u.SZ_DESCRIPTION);

            foreach (var waterShed in waterSheds)
            {
                WaterShedIndex item = new WaterShedIndex()
                {
                    WaterShedDescription = waterShed.SZ_DESCRIPTION == null ? "" : waterShed.SZ_DESCRIPTION,
                    WaterShedID = waterShed.N_LOCATION_TYPE_SYSID
                };
                waterShedList.Add(item);
            }

            return Json(waterShedList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ViewResult Edit(int ID)
        {
            var waterShedEditViewModel = new WaterShedEditViewModel();
            if (ID > 0)
            {
                REF_LOCATION_TYPE_TB waterShed = _uow.Repository<REF_LOCATION_TYPE_TB>().GetById(ID);
                waterShedEditViewModel.WaterShedDescription = waterShed.SZ_DESCRIPTION == null ? "" : waterShed.SZ_DESCRIPTION;
                waterShedEditViewModel.WaterShedID = waterShed.N_LOCATION_TYPE_SYSID;
                waterShedEditViewModel.ShowMessageDescription = false;
                waterShedEditViewModel.MessageDescription = "";
            }
            else
            {
                waterShedEditViewModel.WaterShedDescription = "";
                waterShedEditViewModel.WaterShedID = ID;
                waterShedEditViewModel.ShowMessageDescription = false;
                waterShedEditViewModel.MessageDescription = "";
            }

            return View("Edit", waterShedEditViewModel);
        }

        [HttpPost]
        public ActionResult Edit(WaterShedEditViewModel Model)
        {
            if (_waterShedRepo.IsWaterShedDescriptionDuplicate(Model.WaterShedDescription))
            {
                Model.ShowMessageDescription = true;
                Model.MessageDescription = "The Water Shed Description already exists. Please enter a different one.";

                return View(Model);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    int id = 0;
                    if (Model.WaterShedID == 0) //new
                    {
                        int waterShedID = _waterShedRepo.GetNextWaterShedID();
                        REF_LOCATION_TYPE_TB waterShed = new REF_LOCATION_TYPE_TB()
                        {
                            B_INACTIVE = false,
                            DT_ENTERED = DateTime.UtcNow,
                            DT_MODIFIED = DateTime.UtcNow,
                            SZ_DESCRIPTION = Model.WaterShedDescription,
                            SZ_ENTERED_BY = _modifiedBy,
                            SZ_MODIFIED_BY = _modifiedBy
                        };
                        _uow.Repository<REF_LOCATION_TYPE_TB>().Add(waterShed);
                        _uow.SaveChanges();
                        REF_LOCATION_TYPE_TB waterShedFound = _uow.Repository<REF_LOCATION_TYPE_TB>().Find(u => u.SZ_ENTERED_BY == _modifiedBy
                            && u.SZ_MODIFIED_BY == _modifiedBy && u.SZ_DESCRIPTION == Model.WaterShedDescription
                            ).FirstOrDefault();
                        id = waterShedFound.N_LOCATION_TYPE_SYSID;
                    }
                    else //edit
                    {
                        REF_LOCATION_TYPE_TB waterShed = _uow.Repository<REF_LOCATION_TYPE_TB>().GetById(Model.WaterShedID);
                        waterShed.SZ_MODIFIED_BY = _modifiedBy;
                        waterShed.DT_MODIFIED = DateTime.UtcNow;
                        waterShed.SZ_DESCRIPTION = Model.WaterShedDescription;

                        _uow.Repository<REF_LOCATION_TYPE_TB>().Update(waterShed);
                        _uow.SaveChanges();
                        id = Model.WaterShedID;
                    }

                    return RedirectToAction("Index", new { ID = id });
                }
            }

            return View(Model);
        }

        [HttpGet]
        public ActionResult Delete(string ID)
        {
            try
            {
                REF_LOCATION_TYPE_TB waterShed = _uow.Repository<REF_LOCATION_TYPE_TB>().GetById(Convert.ToInt32(ID));
                WaterShedDeleteViewModel waterShedDeleteViewModel = new WaterShedDeleteViewModel()
                {
                    WaterShedID = waterShed.N_LOCATION_TYPE_SYSID,
                    WaterShedDescription = waterShed.SZ_DESCRIPTION,
                    MessageDescription = "",
                    ShowMessageDescription = false
                };

                return View(waterShedDeleteViewModel);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: WaterShedController.Delete_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: WaterShedController.Delete_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult Delete(WaterShedDeleteViewModel model)
        {
            try
            {
                int id = Convert.ToInt32(model.WaterShedID);
                if (_uow.Repository<REF_LOCATION_TB>().Find(u => u.N_LOCATION_TYPE_SYSID == id).Count() > 0)
                {
                    model.ShowMessageDescription = true;
                    model.MessageDescription = "Location records exist for this water shed. Sorry, the water shed can not be deleted.";

                    return View(model);
                }
                else
                {
                    _uow.Repository<REF_LOCATION_TYPE_TB>().Delete(id);
                    _uow.SaveChanges();

                    return RedirectToAction("Index", new { ID = id });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: WaterShedController.Delete_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: WaterShedController.Delete_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };    
        }
    }
}