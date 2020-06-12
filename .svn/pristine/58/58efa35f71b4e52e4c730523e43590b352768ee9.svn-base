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
    [MacAuthorize(Action="AdministerAll,AdministerOwn")]
    public class FacilityController : BaseController
    {
        private GenericEHWaterQualityUnitOfWork _uow = null;
        private FacilityGet _facilityRepo = null;

        private string _modifiedBy = null;
        private string _userRole = null;
        //private WebServerClient _webServerClient;
        //private HttpClient _client;
        //private Uri _resourceServerUri = new Uri(ConfigurationManager.AppSettings["OAuth.MacombCounty.Paths.Remote.ResourceServerBaseAddress"]);
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public FacilityController()
        {
            _uow = new GenericEHWaterQualityUnitOfWork();
            _facilityRepo = new FacilityGet();
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
        public ActionResult Index(int ID)
        {
            try
            {
                var model = new FacilityIndexViewModel()
                {
                    ID = ID
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: FacilityController.Index_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: FacilityController.Index_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult GetFacilities()
        {
            try
            {
                IEnumerable<REF_FACILITY_TB> facilities = _uow.Repository<REF_FACILITY_TB>().GetAll();
                List<Facility> facilityList = new List<Facility>();

                foreach (var facility in facilities.OrderBy(u => u.SZ_TITLE))
                {
                    Facility item = new Facility()
                    {
                        FacilityID = facility.N_FACILITY_SYSID,
                        FacilityTitle = facility.SZ_TITLE == null ? "" : facility.SZ_TITLE
                    };
                    facilityList.Add(item);
                }

                return Json(facilityList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: FacilityController.GetFacilitites_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: FacilityController.GetFacilities_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult IndexNew()
        {
            try
            {
                return RedirectToAction("Edit", new { ID = 0 });
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: FacilityController.IndexNew_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: FacilityController.IndexNew_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        // GET: /Facility/Edit/5
        [HttpGet]
        public ActionResult Edit(int ID)
        {
            try
            {
                var facilityEditViewModel = new FacilityEditViewModel();
                if (ID > 0)
                {
                    REF_FACILITY_TB facility = _uow.Repository<REF_FACILITY_TB>().GetById(ID);
                    facilityEditViewModel.ID = facility.N_FACILITY_SYSID;
                    facilityEditViewModel.Title = facility.SZ_TITLE;
                    facilityEditViewModel.ShowMessageTitle = false;
                    facilityEditViewModel.MessageTitle = "";
                }
                else
                {
                    facilityEditViewModel.ID = 0;
                    facilityEditViewModel.Title = "";
                    facilityEditViewModel.ShowMessageTitle = false;
                    facilityEditViewModel.MessageTitle = "";
                }

                return View("Edit", facilityEditViewModel);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: FacilityController.Edit_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: FacilityController.Edit_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        // POST: /Facility/Edit/5
        [HttpPost]
        public ActionResult Edit(FacilityEditViewModel Model)
        {
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    if (_facilityRepo.IsFacilityTitleDuplicate(Model.Title))
                    {
                        Model.ShowMessageTitle = true;
                        Model.MessageTitle = "The Facility Title already exists. Please enter a different one.";

                        return View(Model);
                    }
                    else
                    {
                        if (ModelState.IsValid)
                        {
                            int id = 0;
                            if (Model.ID == 0) //new
                            {
                                REF_FACILITY_TB facility = new REF_FACILITY_TB()
                                {
                                    B_INACTIVE = false,
                                    DT_ENTERED = DateTime.UtcNow,
                                    DT_MODIFIED = DateTime.UtcNow,
                                    SZ_ENTERED_BY = _modifiedBy,
                                    SZ_MODIFIED_BY = _modifiedBy,
                                    SZ_TITLE = Model.Title
                                };
                                _uow.Repository<REF_FACILITY_TB>().Add(facility);
                                _uow.SaveChanges();
                                REF_FACILITY_TB facilityFound = _uow.Repository<REF_FACILITY_TB>().Find(u => u.SZ_ENTERED_BY == _modifiedBy
                                    && u.SZ_MODIFIED_BY == _modifiedBy && u.SZ_TITLE == Model.Title).FirstOrDefault();
                                id = facilityFound.N_FACILITY_SYSID;
                            }
                            else //edit
                            {
                                REF_FACILITY_TB facility = _uow.Repository<REF_FACILITY_TB>().GetById(Model.ID);
                                facility.DT_MODIFIED = DateTime.UtcNow;
                                facility.SZ_MODIFIED_BY = _modifiedBy;
                                facility.SZ_TITLE = Model.Title;
                                _uow.Repository<REF_FACILITY_TB>().Update(facility);
                                _uow.SaveChanges();
                                id = Model.ID;
                            }

                            return RedirectToAction("Index", new { ID = id });
                        }

                        return View(Model);
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (var failure in ex.EntityValidationErrors)
                    {
                        sb.AppendFormat("{0} failed validation:\n\n", failure.Entry.Entity.GetType());
                        foreach (var error in failure.ValidationErrors)
                        {
                            sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                            sb.AppendLine();
                        }
                    }
                    if (ex.InnerException == null)
                    {
                        ViewBag.Message = "Function: FacilityController.Edit_POST\n\nError: " + ex.Message + "\n\n" + sb;
                    }
                    else
                    {
                        ViewBag.Message = "Function: FacilityController.Edit_POST\n\nError: " + ex.Message + "\n\n" + sb + "\n\n" + ex.InnerException.Message;
                    };
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    var entry = ex.Entries.Single();
                    entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                }
                catch (DataException ex)
                {
                    if (ex.InnerException == null)
                    {
                        ViewBag.Message = "Function: FacilityController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.GetBaseException().Message;
                    }
                    else
                    {
                        ViewBag.Message = "Function: FacilityController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.GetBaseException().Message + "\n\nInnerException: " + ex.InnerException.Message;
                    };
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        ViewBag.Message = "Function: FacilityController.Edit_POST\n\nError: " + ex.Message;
                    }
                    else
                    {
                        ViewBag.Message = "Function: FacilityController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.InnerException.Message;
                    };
                };
            } while (saveFailed);

            Session["ErrorMessage"] = ViewBag.Message;
            return RedirectToAction("InternalServerError", "Error");
        }

        [HttpGet]
        public ActionResult Delete(FacilityDeleteViewModel model)
        {
            try
            {
                REF_FACILITY_TB facility = _uow.Repository<REF_FACILITY_TB>().GetById(model.ID);
                FacilityDeleteViewModel facilityDeleteViewModel = new FacilityDeleteViewModel()
                {
                    ID = facility.N_FACILITY_SYSID,
                    Title = facility.SZ_TITLE,
                    Message = "",
                    ShowMessage = false
                };

                return View("Delete", facilityDeleteViewModel);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: FacilityController.Delete_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: FacilityController.Delete_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult DeleteFacility(FacilityDeleteViewModel model)
        {
            try
            {
                int id = Convert.ToInt32(model.ID);
                if (_uow.Repository<TRN_SEWER_OVERFLOW_TB>().Find(u => u.N_FACILITY_SYSID == id).Count() > 0)
                {
                    model.ShowMessage = true;
                    model.Message = "Sewer Overflow records exist for this facility. Sorry, the facility can not be deleted.";

                    return View("Delete", model);
                }
                else
                {
                    _uow.Repository<REF_FACILITY_TB>().Delete(id);
                    _uow.SaveChanges();

                    return RedirectToAction("Index", new { ID = id });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: AdminController.Delete_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: AdminController.Delete_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };          
        }
    }    
}