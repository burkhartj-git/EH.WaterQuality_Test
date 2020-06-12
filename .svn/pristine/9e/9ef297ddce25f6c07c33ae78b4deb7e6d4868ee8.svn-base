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
    public class WaterBodyController : BaseController
    {
        private GenericEHWaterQualityUnitOfWork _uow = null;
        private WaterBodyGet _waterBodyRepo = null;

        private string _modifiedBy = null;
        private string _userRole = null;
        //private WebServerClient _webServerClient;
        //private HttpClient _client;
        //private Uri _resourceServerUri = new Uri(ConfigurationManager.AppSettings["OAuth.MacombCounty.Paths.Remote.ResourceServerBaseAddress"]);

        public WaterBodyController()
        {
            _uow = new GenericEHWaterQualityUnitOfWork();
            _waterBodyRepo = new WaterBodyGet();

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
            var model = new WaterBodyIndexViewModel()
            {
                ID = ID
            };

            return View(model);
        }

        [HttpGet]
        public JsonResult GetWaterBodies()
        {
            IEnumerable<REF_WATER_BODY_TB> waterBodies = _uow.Repository<REF_WATER_BODY_TB>().GetAll();
            List<WaterBodyIndex> waterBodyList = new List<WaterBodyIndex>();

            waterBodies = waterBodies.ToList().OrderBy(u => u.SZ_DESCRIPTION);

            foreach (var waterBody in waterBodies)
            {
                WaterBodyIndex item = new WaterBodyIndex()
                {
                    WaterBodyDescription = waterBody.SZ_DESCRIPTION == null ? "" : waterBody.SZ_DESCRIPTION,
                    WaterBodyID = waterBody.N_WATER_BODY_SYSID,
                    WaterBodyName = waterBody.SZ_NAME == null ? "" : waterBody.SZ_NAME
                };
                waterBodyList.Add(item);
            }

            return Json(waterBodyList, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ViewResult Edit(int ID)
        {
            var waterBodyEditViewModel = new WaterBodyEditViewModel();
            if (ID > 0)
            {
                REF_WATER_BODY_TB waterBody = _uow.Repository<REF_WATER_BODY_TB>().GetById(ID);
                waterBodyEditViewModel.WaterBodyDescription = waterBody.SZ_DESCRIPTION == null ? "" : waterBody.SZ_DESCRIPTION;
                waterBodyEditViewModel.WaterBodyID = waterBody.N_WATER_BODY_SYSID;
                waterBodyEditViewModel.WaterBodyName = waterBody.SZ_NAME == null ? "" : waterBody.SZ_NAME;
                waterBodyEditViewModel.ShowMessageNameAndDescription = false;
                waterBodyEditViewModel.MessageNameAndDescription = "";
            }
            else
            {
                waterBodyEditViewModel.WaterBodyDescription = "";
                waterBodyEditViewModel.WaterBodyID = ID;
                waterBodyEditViewModel.WaterBodyName = "";
                waterBodyEditViewModel.ShowMessageNameAndDescription = false;
                waterBodyEditViewModel.MessageNameAndDescription = "";
            }

            return View("Edit", waterBodyEditViewModel);
        }

        [HttpPost]
        public ActionResult Edit(WaterBodyEditViewModel Model)
        {
            if (_waterBodyRepo.IsWaterBodyNameAndDescriptionDuplicate(Model.WaterBodyName, Model.WaterBodyDescription))
            {
                Model.ShowMessageNameAndDescription = true;
                Model.MessageNameAndDescription = "The Water Body Name and Description combination already exists. Please enter a different one.";

                return View(Model);
            }
            else
            {
                if (ModelState.IsValid)
                {
                    int id = 0;
                    if (Model.WaterBodyID == 0) //new
                    {
                        REF_WATER_BODY_TB waterBody = new REF_WATER_BODY_TB()
                        {
                            B_INACTIVE = false,
                            DT_ENTERED = DateTime.UtcNow,
                            DT_MODIFIED = DateTime.UtcNow,
                            SZ_ENTERED_BY = _modifiedBy,
                            SZ_MODIFIED_BY = _modifiedBy,
                            SZ_DESCRIPTION = Model.WaterBodyDescription,
                            SZ_NAME = Model.WaterBodyName
                        };
                        _uow.Repository<REF_WATER_BODY_TB>().Add(waterBody);
                        _uow.SaveChanges();
                        REF_WATER_BODY_TB waterBodyFound = _uow.Repository<REF_WATER_BODY_TB>().Find(u => u.SZ_ENTERED_BY == _modifiedBy
                            && u.SZ_MODIFIED_BY == _modifiedBy && u.SZ_DESCRIPTION == Model.WaterBodyDescription
                            && u.SZ_NAME == Model.WaterBodyName).FirstOrDefault();
                        id = waterBodyFound.N_WATER_BODY_SYSID;
                    }
                    else //edit
                    {
                        REF_WATER_BODY_TB waterBody = _uow.Repository<REF_WATER_BODY_TB>().GetById(Model.WaterBodyID);
                        waterBody.DT_MODIFIED = DateTime.UtcNow;
                        waterBody.SZ_DESCRIPTION = Model.WaterBodyDescription;
                        waterBody.SZ_MODIFIED_BY = _modifiedBy;
                        waterBody.SZ_NAME = Model.WaterBodyName;

                        _uow.Repository<REF_WATER_BODY_TB>().Update(waterBody);
                        _uow.SaveChanges();
                        id = Model.WaterBodyID;
                    }

                    return RedirectToAction("Index", new { ID = id });
                }

                return View(Model);
            }
        }

        [HttpGet]
        public ActionResult Delete(string ID)
        {
            try
            {
                REF_WATER_BODY_TB waterBody = _uow.Repository<REF_WATER_BODY_TB>().GetById(Convert.ToInt32(ID));
                WaterBodyDeleteViewModel waterBodyDeleteViewModel = new WaterBodyDeleteViewModel()
                {
                    WaterBodyID = waterBody.N_WATER_BODY_SYSID,
                    WaterBodyName = waterBody.SZ_NAME,
                    WaterBodyDescription = waterBody.SZ_DESCRIPTION,
                    MessageNameAndDescription = "",
                    ShowMessageNameAndDescription = false
                };

                return View(waterBodyDeleteViewModel);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: WaterBodyController.Delete_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: WaterBodyController.Delete_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult Delete(WaterBodyDeleteViewModel model)
        {
            try
            {
                int id = Convert.ToInt32(model.WaterBodyID);
                if (_uow.Repository<REF_LOCATION_TB>().Find(u => u.N_WATER_BODY_SYSID == id).Count() > 0 ||
                    _uow.Repository<TRN_SEWER_OVERFLOW_TB>().Find(u => u.N_WATER_BODY_SYSID == id).Count() > 0)
                {
                    model.ShowMessageNameAndDescription = true;
                    model.MessageNameAndDescription = "Records exist for this water body. Sorry, the water body can not be deleted.";

                    return View(model);
                }
                else
                {
                    _uow.Repository<REF_WATER_BODY_TB>().Delete(id);
                    _uow.SaveChanges();

                    return RedirectToAction("Index", new { ID = id });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: WaterBodyController.Delete_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: WaterBodyController.Delete_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };   
        }
    }
}