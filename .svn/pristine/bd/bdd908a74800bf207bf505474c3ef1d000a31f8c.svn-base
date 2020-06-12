using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
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
    public class LocationGroupController : BaseController
    {
        private GenericEHWaterQualityUnitOfWork _uow = null;
        private LocationGet _locationRepo = null;

        private string _modifiedBy = null;
        private string _userRole = null;
        //private WebServerClient _webServerClient;
        //private HttpClient _client;
        //private Uri _resourceServerUri = new Uri(ConfigurationManager.AppSettings["OAuth.MacombCounty.Paths.Remote.ResourceServerBaseAddress"]);

        public LocationGroupController()
        {
            _uow = new GenericEHWaterQualityUnitOfWork();
            _locationRepo = new LocationGet();

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
                    ViewBag.Message = "Function: LocationGroupController.Index_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: LocationGroupController.Index_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult GetLocationGroups()
        {
            try
            {
                IEnumerable<REF_LOCATION_GROUP_TB> locationGroups = _uow.Repository<REF_LOCATION_GROUP_TB>().GetAll();
                List<LocationGroupIndex> locationGroupList = new List<LocationGroupIndex>();

                locationGroups = locationGroups.ToList().OrderBy(u => u.SZ_DESCRIPTION);

                foreach (var locationGroup in locationGroups)
                {
                    LocationGroupIndex item = new LocationGroupIndex()
                    {
                        Description = locationGroup.SZ_DESCRIPTION,
                        ID = locationGroup.N_LOCATION_GROUP_SYSID
                    };
                    locationGroupList.Add(item);
                }

                return Json(locationGroupList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: LocationGroupController.GetLocationGroups_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: LocationGroupController.GetLocationGroups_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult Edit(int ID)
        {
            try
            {
                var locationGroupEditViewModel = new LocationGroupEditViewModel();
                if (ID > 0)
                {
                    REF_LOCATION_GROUP_TB locationGroup = _uow.Repository<REF_LOCATION_GROUP_TB>().GetById(ID);
                    locationGroupEditViewModel.Description = locationGroup.SZ_DESCRIPTION;
                    locationGroupEditViewModel.ID = locationGroup.N_LOCATION_GROUP_SYSID;
                    locationGroupEditViewModel.ShowMessageDescription = false;
                    locationGroupEditViewModel.MessageDescription = "";
                }
                else
                {
                    locationGroupEditViewModel.Description = "";
                    locationGroupEditViewModel.ID = 0;
                    locationGroupEditViewModel.ShowMessageDescription = false;
                    locationGroupEditViewModel.MessageDescription = "";
                }

                return View("Edit", locationGroupEditViewModel);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: LocationGroupController.Edit_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: LocationGroupController.Edit_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult Edit(LocationGroupEditViewModel Model)
        {
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    if (_locationRepo.IsLocationGroupDescriptionDuplicate(Model.Description))
                    {
                        Model.ShowMessageDescription = true;
                        Model.MessageDescription = "The Location Group Description already exists. Please enter a different one.";

                        return View(Model);
                    }
                    else
                    {
                        if (ModelState.IsValid)
                        {
                            int id = 0;
                            if (Model.ID == 0) //new
                            {
                                REF_LOCATION_GROUP_TB locationGroup = new REF_LOCATION_GROUP_TB()
                                {
                                    B_INACTIVE = false,
                                    DT_ENTERED = DateTime.UtcNow,
                                    DT_MODIFIED = DateTime.UtcNow,
                                    SZ_DESCRIPTION = Model.Description,
                                    SZ_ENTERED_BY = _modifiedBy,
                                    SZ_MODIFIED_BY = _modifiedBy
                                };
                                _uow.Repository<REF_LOCATION_GROUP_TB>().Add(locationGroup);
                                _uow.SaveChanges();
                                REF_LOCATION_GROUP_TB locationGroupFound = _uow.Repository<REF_LOCATION_GROUP_TB>().Find(u => u.SZ_ENTERED_BY == _modifiedBy
                                    && u.SZ_MODIFIED_BY == _modifiedBy && u.SZ_DESCRIPTION == Model.Description).FirstOrDefault();
                                id = locationGroupFound.N_LOCATION_GROUP_SYSID;
                            }
                            else //edit
                            {
                                REF_LOCATION_GROUP_TB locationGroup = _uow.Repository<REF_LOCATION_GROUP_TB>().GetById(Model.ID);
                                locationGroup.DT_MODIFIED = DateTime.UtcNow;
                                locationGroup.SZ_DESCRIPTION = Model.Description;
                                locationGroup.SZ_MODIFIED_BY = _modifiedBy;
                                _uow.Repository<REF_LOCATION_GROUP_TB>().Update(locationGroup);
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
                        ViewBag.Message = "Function: LocationGroupController.Edit_POST\n\nError: " + ex.Message + "\n\n" + sb;
                    }
                    else
                    {
                        ViewBag.Message = "Function: LocationGroupController.Edit_POST\n\nError: " + ex.Message + "\n\n" + sb + "\n\n" + ex.InnerException.Message;
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
                        ViewBag.Message = "Function: LocationGroupController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.GetBaseException().Message;
                    }
                    else
                    {
                        ViewBag.Message = "Function: LocationGroupController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.GetBaseException().Message + "\n\nInnerException: " + ex.InnerException.Message;
                    };
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        ViewBag.Message = "Function: LocationGroupController.Edit_POST\n\nError: " + ex.Message;
                    }
                    else
                    {
                        ViewBag.Message = "Function: LocationGroupController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.InnerException.Message;
                    };
                };
            } while (saveFailed);

            Session["ErrorMessage"] = ViewBag.Message;
            return RedirectToAction("InternalServerError", "Error");
        }

        [HttpGet]
        public ActionResult Details(int ID)
        {
            try
            {
                REF_LOCATION_GROUP_TB locationGroup = _uow.Repository<REF_LOCATION_GROUP_TB>().GetById(ID);

                LocationGroupDetailsViewModel model = new LocationGroupDetailsViewModel()
                {
                    Description = locationGroup.SZ_DESCRIPTION,
                    LocationGroupID = locationGroup.N_LOCATION_GROUP_SYSID
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: LocationGroupController.Details_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: LocationGroupController.Details_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult GetLocations(string LocationGroupID)
        {
            try
            {
                int locGroupID = Convert.ToInt32(LocationGroupID);
                IEnumerable<REF_LOCATION_X_LOCATION_GROUP_TB> locGroupByLoc = _locationRepo.GetLocationsByLocationGroupID(locGroupID);
                List<LocationDetails> locDetails = new List<LocationDetails>();

                foreach (var row in locGroupByLoc)
                {
                    LocationDetails item = new LocationDetails()
                    {
                        LocationDescription = row.REF_LOCATION_TB.SZ_LABEL + " - " + row.REF_LOCATION_TB.SZ_DESCRIPTION,
                        LocationGroupID = row.N_LOCATION_GROUP_SYSID.ToString(),
                        LocationID = row.N_LOCATION_SYSID
                    };
                    locDetails.Add(item);
                }

                return Json(locDetails, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: LocationGroupController.GetLocations_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: LocationGroupController.GetLocations_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult Delete(int ID, string LocationGroupID)
        {
            try
            {
                bool isDeleted = _locationRepo.DeleteLocByLocGroupByIDs(ID, Convert.ToInt32(LocationGroupID));

                int locGroupID = Convert.ToInt32(LocationGroupID);

                return RedirectToAction("Details", new { ID = locGroupID });
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: LocationGroupController.Delete_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: LocationGroupController.Delete_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult Create(int ID, int? LocationGroupID)
        {
            try
            {
                REF_LOCATION_GROUP_TB locationGroup = _uow.Repository<REF_LOCATION_GROUP_TB>().GetById(Convert.ToInt32(LocationGroupID));

                LocationGroupCreateLocationViewModel model = new LocationGroupCreateLocationViewModel()
                {
                    LocationGroupDescription = locationGroup.SZ_DESCRIPTION,
                    LocationGroupID = locationGroup.N_LOCATION_GROUP_SYSID,
                    Locations = (List<SelectListItem>)_locationRepo.GetLocationsSelectListByLocationGroupID(Convert.ToInt32(LocationGroupID)),
                    SelectedLocation = 0
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: LocationGroupController.Create_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: LocationGroupController.Create_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult Create(LocationGroupCreateLocationViewModel Model, bool IsTest = false)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    REF_LOCATION_X_LOCATION_GROUP_TB record = new REF_LOCATION_X_LOCATION_GROUP_TB()
                    {
                        B_INACTIVE = false,
                        DT_ENTERED = DateTime.UtcNow,
                        DT_MODIFIED = DateTime.UtcNow,
                        N_LOCATION_GROUP_SYSID = Model.LocationGroupID,
                        N_LOCATION_SYSID = Model.SelectedLocation,
                        SZ_ENTERED_BY = IsTest ? "Unit Test Case" : _modifiedBy,
                        SZ_MODIFIED_BY = _modifiedBy
                    };

                    _uow.Repository<REF_LOCATION_X_LOCATION_GROUP_TB>().Add(record);
                    _uow.SaveChanges();

                    return RedirectToAction("Details", new { ID = Model.LocationGroupID });
                }

                Model.Locations = (List<SelectListItem>)_locationRepo.GetLocationsSelectListByLocationGroupID(Model.LocationGroupID);

                return View(Model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: LocationGroupController.Create_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: LocationGroupController.Craete_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult DeleteLocGr(string ID)
        {
            try
            {
                REF_LOCATION_GROUP_TB locationGroup = _uow.Repository<REF_LOCATION_GROUP_TB>().GetById(Convert.ToInt32(ID));
                LocationGroupDeleteViewModel locationGroupDeleteViewModel = new LocationGroupDeleteViewModel()
                {
                    Description = locationGroup.SZ_DESCRIPTION,
                    ID = locationGroup.N_LOCATION_GROUP_SYSID,
                    MessageDescription = "",
                    ShowMessageDescription = false
                };

                return View(locationGroupDeleteViewModel);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: LocationGroupController.DeleteLocGr_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: LocationGroupController.DeleteLocGr_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult DeleteLocGr(LocationGroupDeleteViewModel model)
        {
            try
            {
                int id = Convert.ToInt32(model.ID);
                if (_uow.Repository<REF_LOCATION_X_LOCATION_GROUP_TB>().Find(u => u.N_LOCATION_GROUP_SYSID == id).Count() > 0 ||
                    _uow.Repository<REF_REQUEST_TB>().Find(u => u.N_LOCATION_GROUP_SYSID == id).Count() > 0)
                {
                    model.ShowMessageDescription = true;
                    model.MessageDescription = "Records exist for this location group. Sorry, the location group can not be deleted.";

                    return View(model);
                }
                else
                {
                    _uow.Repository<REF_LOCATION_GROUP_TB>().Delete(id);
                    _uow.SaveChanges();

                    return RedirectToAction("Index", new { ID = id });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: LocationGroupController.DeleteLocGr_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: LocationGroupController.DeleteLocGr_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };      
        }
    }
}