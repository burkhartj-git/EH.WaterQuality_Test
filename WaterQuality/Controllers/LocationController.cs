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
    public class LocationController : BaseController
    {
        private GenericEHWaterQualityUnitOfWork _uow = null;
        private LocationGet _locationRepo = null;
        private WaterBodyGet _waterBodyRepo = null;
        private WaterShedGet _waterShedRepo = null;

        private string _modifiedBy = null;
        private string _userRole = null;
        //private WebServerClient _webServerClient;
        //private HttpClient _client;
        //private Uri _resourceServerUri = new Uri(ConfigurationManager.AppSettings["OAuth.MacombCounty.Paths.Remote.ResourceServerBaseAddress"]);

        public LocationController()
        {
            _uow = new GenericEHWaterQualityUnitOfWork();
            _locationRepo = new LocationGet();
            _waterBodyRepo = new WaterBodyGet();
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
        public ActionResult Index(int ID, string Search = "all")
        {
            try
            {
                string search;
                if (Search == "")
                {
                    search = "all";
                }
                else
                {
                    search = Search;
                }
                var model = new LocationIndexViewModel()
                {
                    ID = ID,
                    Search = search
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: LocationController.Index_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: LocationController.Index_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult GetLocations(string SearchString, string SearchOrder)
        {
            try
            {
                IEnumerable<REF_LOCATION_TB> locations = _uow.Repository<REF_LOCATION_TB>().GetAll();
                List<LocationIndex> locationList = new List<LocationIndex>();

                if (SearchString != "" && SearchString != "all")
                {
                    locations = locations.ToList().Where(u => u.SZ_DESCRIPTION.ToLower().Contains(SearchString.ToLower()));
                }

                if (SearchOrder == "Asc")
                {
                    locations = locations.ToList().OrderBy(u => u.SZ_LABEL).ThenBy(u => u.SZ_DESCRIPTION);
                }
                else
                {
                    locations = locations.ToList().OrderByDescending(u => u.SZ_LABEL).ThenBy(u => u.SZ_DESCRIPTION);
                }

                foreach (var location in locations)
                {
                    LocationIndex item = new LocationIndex()
                    {
                        EffectiveDate = location.DT_EFFECTIVE == DateTime.MinValue || location.DT_EFFECTIVE == null ? "" : location.DT_EFFECTIVE.Value.ToShortDateString(),
                        ExpiredDate = location.DT_EXPIRED == DateTime.MinValue || location.DT_EXPIRED == null ? "" : location.DT_EXPIRED.Value.ToShortDateString(),
                        LocationDescription = location.SZ_DESCRIPTION == null ? "" : location.SZ_DESCRIPTION,
                        LocationID = location.N_LOCATION_SYSID,
                        SiteID = location.SZ_LABEL == null ? "" : location.SZ_LABEL,
                        WaterBody = location.REF_WATER_BODY_TB == null ? "" : location.REF_WATER_BODY_TB.SZ_DESCRIPTION,
                        WaterShed = location.REF_LOCATION_TYPE_TB == null ? "" : location.REF_LOCATION_TYPE_TB.SZ_DESCRIPTION,
                        XCoordinate = location.N_GIS_X == null ? "" : location.N_GIS_X.ToString(),
                        YCoordinate = location.N_GIS_Y == null ? "" : location.N_GIS_Y.ToString(),
                        OrderUpDown = location.SZ_STREAM_NUMBER == null ? "" : location.SZ_STREAM_NUMBER
                    };
                    locationList.Add(item);
                }

                return Json(locationList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: LocationController.GetLocations_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: LocationController.GetLocations_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult Edit(int ID, string SearchText)
        {
            try
            {
                var locationEditViewModel = new LocationEditViewModel();
                if (ID > 0)
                {
                    REF_LOCATION_TB location = _uow.Repository<REF_LOCATION_TB>().GetById(ID);
                    locationEditViewModel.Description = location.SZ_DESCRIPTION;
                    locationEditViewModel.EffectiveDate = location.DT_EFFECTIVE == null ? "" : location.DT_EFFECTIVE.Value.ToShortDateString();
                    locationEditViewModel.ExpiredDate = location.DT_EXPIRED == null ? "" : location.DT_EXPIRED.Value.ToShortDateString();
                    locationEditViewModel.LocationID = ID;
                    locationEditViewModel.SearchText = SearchText;
                    locationEditViewModel.SelectedWaterBody = Convert.ToInt32(location.N_WATER_BODY_SYSID);
                    locationEditViewModel.SelectedWaterShed = Convert.ToInt32(location.N_LOCATION_TYPE_SYSID);
                    locationEditViewModel.Station = location.SZ_LABEL;
                    locationEditViewModel.WaterBodies = (List<SelectListItem>)_waterBodyRepo.GetWaterBodies();
                    locationEditViewModel.WaterSheds = (List<SelectListItem>)_waterShedRepo.GetWaterSheds();
                    locationEditViewModel.XCoordinate = Convert.ToDouble(location.N_GIS_X);
                    locationEditViewModel.YCoordinate = Convert.ToDouble(location.N_GIS_Y);
                    locationEditViewModel.OrderUpDown = location.SZ_STREAM_NUMBER;
                }
                else
                {
                    locationEditViewModel.Description = "";
                    locationEditViewModel.EffectiveDate = "";
                    locationEditViewModel.ExpiredDate = "";
                    locationEditViewModel.LocationID = ID;
                    locationEditViewModel.SearchText = SearchText;
                    locationEditViewModel.SelectedWaterBody = 0;
                    locationEditViewModel.SelectedWaterShed = 0;
                    locationEditViewModel.Station = "";
                    locationEditViewModel.WaterBodies = (List<SelectListItem>)_waterBodyRepo.GetWaterBodies();
                    locationEditViewModel.WaterSheds = (List<SelectListItem>)_waterShedRepo.GetWaterSheds();
                    locationEditViewModel.XCoordinate = 0;
                    locationEditViewModel.YCoordinate = 0;
                    locationEditViewModel.OrderUpDown = "";
                }

                return View("Edit", locationEditViewModel);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: LocationController.Edit_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: LocationController.Edit_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult Edit(LocationEditViewModel Model)
        {
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    if (ModelState["XCoordinate"].Errors.Count > 0)
                    {
                        string code = ModelState["XCoordinate"].Value.AttemptedValue.ToString();
                        string[] parts = code.Split(',');
                        string entry = parts[0];
                        string response = "The value: '" + entry + "' is not valid. Please enter a numeric value.";
                        ModelState.Remove("XCoordinate");
                        ModelState.AddModelError("XCoordinate", response);
                    }

                    if (ModelState["YCoordinate"].Errors.Count > 0)
                    {
                        string code = ModelState["YCoordinate"].Value.AttemptedValue.ToString();
                        string[] parts = code.Split(',');
                        string entry = parts[0];
                        string response = "The value: '" + entry + "' is not valid. Please enter a numeric value.";
                        ModelState.Remove("YCoordinate");
                        ModelState.AddModelError("YCoordinate", response);
                    }
                    if (ModelState["SelectedWaterBody"].Errors.Count > 0)
                    {
                        Model.SelectedWaterBody = null;
                        ModelState.Remove("SelectedWaterBody");
                    }
                    if (ModelState["SelectedWaterShed"].Errors.Count > 0)
                    {
                        Model.SelectedWaterShed = null;
                        ModelState.Remove("SelectedWaterShed");
                    }

                    if (ModelState.IsValid)
                    {
                        int id = 0;
                        if (Model.LocationID == 0) //new
                        {
                            REF_LOCATION_TB location = new REF_LOCATION_TB()
                            {
                                B_INACTIVE = false,
                                DT_EFFECTIVE = Model.EffectiveDate == "" ? (DateTime?)null : Convert.ToDateTime(Model.EffectiveDate),
                                DT_ENTERED = DateTime.UtcNow,
                                DT_EXPIRED = Model.ExpiredDate == "" ? (DateTime?)null : Convert.ToDateTime(Model.ExpiredDate),
                                DT_MODIFIED = DateTime.UtcNow,
                                N_GIS_X = Convert.ToDecimal(Model.XCoordinate),
                                N_GIS_Y = Convert.ToDecimal(Model.YCoordinate),
                                N_LOCATION_TYPE_SYSID = Model.SelectedWaterShed,
                                N_WATER_BODY_SYSID = Model.SelectedWaterBody,
                                SZ_DESCRIPTION = Model.Description,
                                SZ_ENTERED_BY = _modifiedBy,
                                SZ_LABEL = Model.Station,
                                SZ_MODIFIED_BY = _modifiedBy,
                                SZ_STREAM_NUMBER = Model.OrderUpDown
                            };
                            _uow.Repository<REF_LOCATION_TB>().Add(location);
                            _uow.SaveChanges();
                            DateTime effectiveDate = Convert.ToDateTime(Model.EffectiveDate);
                            DateTime expiredDate = Convert.ToDateTime(Model.ExpiredDate);
                            Decimal xCoord = Convert.ToDecimal(Model.XCoordinate);
                            Decimal yCoord = Convert.ToDecimal(Model.YCoordinate);
                            REF_LOCATION_TB locationFound = _uow.Repository<REF_LOCATION_TB>().Find(u => u.SZ_ENTERED_BY == _modifiedBy
                                && u.SZ_MODIFIED_BY == _modifiedBy && u.DT_EFFECTIVE == effectiveDate && u.DT_EXPIRED == expiredDate
                                && u.N_GIS_X == xCoord && u.N_GIS_Y == yCoord && u.N_LOCATION_TYPE_SYSID == Model.SelectedWaterShed
                                && u.N_WATER_BODY_SYSID == Model.SelectedWaterBody && u.SZ_DESCRIPTION == Model.Description
                                && u.SZ_LABEL == Model.Station && u.SZ_STREAM_NUMBER == Model.OrderUpDown).FirstOrDefault();
                            id = locationFound.N_LOCATION_SYSID;
                        }
                        else //edit
                        {
                            REF_LOCATION_TB location = _uow.Repository<REF_LOCATION_TB>().GetById(Model.LocationID);
                            location.DT_EFFECTIVE = Convert.ToDateTime(Model.EffectiveDate);
                            location.DT_EXPIRED = Convert.ToDateTime(Model.ExpiredDate);
                            location.DT_MODIFIED = DateTime.UtcNow;
                            location.N_GIS_X = Convert.ToDecimal(Model.XCoordinate);
                            location.N_GIS_Y = Convert.ToDecimal(Model.YCoordinate);
                            location.N_LOCATION_TYPE_SYSID = Model.SelectedWaterShed;
                            location.N_WATER_BODY_SYSID = Model.SelectedWaterBody;
                            location.SZ_DESCRIPTION = Model.Description;
                            location.SZ_LABEL = Model.Station;
                            location.SZ_STREAM_NUMBER = Model.OrderUpDown;
                            _uow.Repository<REF_LOCATION_TB>().Update(location);
                            _uow.SaveChanges();
                            id = Model.LocationID;
                        }

                        return RedirectToAction("Index", new { ID = id, Search = Model.SearchText });
                    }

                    Model.WaterBodies = (List<SelectListItem>)_waterBodyRepo.GetWaterBodies();
                    Model.WaterSheds = (List<SelectListItem>)_waterShedRepo.GetWaterSheds();

                    return View(Model);
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
                        ViewBag.Message = "Function: LocationController.Edit_POST\n\nError: " + ex.Message + "\n\n" + sb;
                    }
                    else
                    {
                        ViewBag.Message = "Function: LocationController.Edit_POST\n\nError: " + ex.Message + "\n\n" + sb + "\n\n" + ex.InnerException.Message;
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
                        ViewBag.Message = "Function: LocationController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.GetBaseException().Message;
                    }
                    else
                    {
                        ViewBag.Message = "Function: LocationController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.GetBaseException().Message + "\n\nInnerException: " + ex.InnerException.Message;
                    };
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        ViewBag.Message = "Function: LocationController.Edit_POST\n\nError: " + ex.Message;
                    }
                    else
                    {
                        ViewBag.Message = "Function: LocationController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.InnerException.Message;
                    };
                };
            } while (saveFailed);

            Session["ErrorMessage"] = ViewBag.Message;
            return RedirectToAction("InternalServerError", "Error");
        }

        [HttpGet]
        public ActionResult Details(int ID, string SearchText, int? LocationGroupID)
        {
            try
            {
                REF_LOCATION_TB location = _uow.Repository<REF_LOCATION_TB>().GetById(ID);

                LocationDetailsViewModel model = new LocationDetailsViewModel()
                {
                    Description = location.SZ_DESCRIPTION,
                    EffectiveDate = location.DT_EFFECTIVE == null ? "" : location.DT_EFFECTIVE.Value.ToShortDateString(),
                    ExpiredDate = location.DT_EXPIRED == null ? "" : location.DT_EXPIRED.Value.ToShortDateString(),
                    LocationGroupID = Convert.ToInt32(LocationGroupID),
                    LocationID = ID,
                    SearchText = SearchText,
                    Station = location.SZ_LABEL,
                    XCoordinate = location.N_GIS_X.ToString(),
                    YCoordinate = location.N_GIS_Y.ToString(),
                    OrderUpDown = location.SZ_STREAM_NUMBER
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: LocationController.Details_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: LocationController.Details_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult GetLocationGroups(string LocationID)
        {
            try
            {
                int locID = Convert.ToInt32(LocationID);
                IEnumerable<REF_LOCATION_X_LOCATION_GROUP_TB> locByLocGroup = _locationRepo.GetLocationGroupsByLocationID(locID);
                List<LocationDetails> locDetails = new List<LocationDetails>();

                foreach (var row in locByLocGroup)
                {
                    LocationDetails item = new LocationDetails()
                    {
                        LocationGroupDescription = row.REF_LOCATION_GROUP_TB.SZ_DESCRIPTION,
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
                    ViewBag.Message = "Function: LocationController.GetLocationGroups_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: LocationController.GetLocationGroups_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult Delete(int ID, string SearchText, string LocationGroupID)
        {
            try
            {
                bool isDeleted = _locationRepo.DeleteLocByLocGroupByIDs(ID, Convert.ToInt32(LocationGroupID));
        
                return RedirectToAction("Details", new { ID = ID, SearchText = SearchText, LocationGroupID = 0 });
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: LocationController.Delete_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: LocationController.Delete_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult Create(int ID, string SearchText, int? LocationGroupID)
        {
            try
            {
                REF_LOCATION_TB location = _uow.Repository<REF_LOCATION_TB>().GetById(ID);

                LocationCreateGroupViewModel model = new LocationCreateGroupViewModel()
                {
                    ID = ID,
                    LocationDescription = location.SZ_LABEL + " - " + location.SZ_DESCRIPTION,
                    LocationGroups = (List<SelectListItem>)_locationRepo.GetLocationGroupsSelectListByLocationID(ID),
                    SelectedLocationGroup = 0,
                    SearchText = SearchText
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: LocationController.Create_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: LocationController.Create_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult Create(LocationCreateGroupViewModel Model, bool IsTest = false)
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
                        N_LOCATION_GROUP_SYSID = Model.SelectedLocationGroup,
                        N_LOCATION_SYSID = Model.ID,
                        SZ_ENTERED_BY = IsTest ? "Unit Test Case" : _modifiedBy,
                        SZ_MODIFIED_BY = _modifiedBy
                    };

                    _uow.Repository<REF_LOCATION_X_LOCATION_GROUP_TB>().Add(record);
                    _uow.SaveChanges();

                    return RedirectToAction("Details", new { ID = Model.ID, SearchText = Model.SearchText, LocationGroupID = Model.SelectedLocationGroup });
                }

                Model.LocationGroups = (List<SelectListItem>)_locationRepo.GetLocationGroupsSelectListByLocationID(Model.ID);

                return View(Model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: LocationController.Create_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: LocationController.Create_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult DeleteLoc(string ID, string SearchText)
        {
            try
            {
                int id = Convert.ToInt32(ID);
                REF_LOCATION_TB location = _uow.Repository<REF_LOCATION_TB>().GetById(id);
                LocationDeleteViewModel locationDeleteViewModel = new LocationDeleteViewModel()
                {
                    Description = location.SZ_DESCRIPTION,
                    EffectiveDate = location.DT_EFFECTIVE == null ? "" : location.DT_EFFECTIVE.Value.ToShortDateString(),
                    ExpiredDate = location.DT_EXPIRED == null ? "" : location.DT_EXPIRED.Value.ToShortDateString(),
                    LocationID = location.N_LOCATION_SYSID,
                    Message = "",
                    SelectedWaterBody = Convert.ToInt32(location.N_WATER_BODY_SYSID),
                    SelectedWaterShed = Convert.ToInt32(location.N_LOCATION_TYPE_SYSID),
                    ShowMessage = false,
                    Station = location.SZ_LABEL,
                    XCoordinate = location.N_GIS_X.ToString(),
                    YCoordinate = location.N_GIS_Y.ToString(),
                    WaterBody = _waterBodyRepo.GetWaterBodyByID(Convert.ToInt32(location.N_WATER_BODY_SYSID)),
                    WaterShed = _waterShedRepo.GetWaterShedDescriptionByID(Convert.ToInt32(location.N_LOCATION_TYPE_SYSID)),
                    OrderUpDown = location.SZ_STREAM_NUMBER,
                    SearchText = SearchText
                };

                return View(locationDeleteViewModel);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: LocationController.DeleteLoc_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: LocationController.DeleteLoc_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult DeleteLoc(LocationDeleteViewModel model)
        {
            try
            {
                int id = Convert.ToInt32(model.LocationID);
                if (_uow.Repository<TRN_SAMPLE_TB>().Find(u => u.N_LOCATION_SYSID == id).Count() > 0)
                {
                    model.ShowMessage = true;
                    model.Message = "Sample records exist for this location. Sorry, the location can not be deleted.";

                    return View("DeleteLoc", model);
                }
                else
                {
                    _uow.Repository<REF_LOCATION_TB>().Delete(id);
                    _uow.SaveChanges();

                    return RedirectToAction("Index", new { ID = id });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: LocationController.DeleteLoc_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: LocationController.DeleteLoc_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };     
        }
    }
}