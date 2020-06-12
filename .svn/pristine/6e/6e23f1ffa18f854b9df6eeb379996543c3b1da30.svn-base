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
    public class RequestController : BaseController
    {
        private GenericEHWaterQualityUnitOfWork _uow = null;
        private LocationGet _locationRepo = null;
        private RequestGroupGet _requestRepo = null;
        private TestGet _testRepo = null;

        private string _modifiedBy = null;
        private string _userRole = null;
        //private WebServerClient _webServerClient;
        //private HttpClient _client;
        //private Uri _resourceServerUri = new Uri(ConfigurationManager.AppSettings["OAuth.MacombCounty.Paths.Remote.ResourceServerBaseAddress"]);

        public RequestController()
        {
            _uow = new GenericEHWaterQualityUnitOfWork();
            _locationRepo = new LocationGet();
            _requestRepo = new RequestGroupGet();
            _testRepo = new TestGet();

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
                var model = new RequestIndexViewModel()
                {
                    ID = ID
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: RequestController.Index_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: RequestController.Index_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult GetRequests()
        {
            try
            {
                IEnumerable<REF_REQUEST_TB> requests = _uow.Repository<REF_REQUEST_TB>().GetAll();
                List<RequestIndex> requestList = new List<RequestIndex>();

                requests = requests.ToList().OrderBy(u => u.REF_REQUEST_GROUP_TB.SZ_DESCRIPTION)
                                            .ThenBy(u => u.REF_LOCATION_GROUP_TB.SZ_DESCRIPTION)
                                            .ThenBy(u => u.REF_TEST_GROUP_TB.SZ_DESCRIPTION);

                foreach (var request in requests)
                {
                    RequestIndex item = new RequestIndex()
                    {
                        RequestID = request.N_REQUEST_SYSID,
                        LocationGroupDescription = request.REF_LOCATION_GROUP_TB.SZ_DESCRIPTION == null ? "" : request.REF_LOCATION_GROUP_TB.SZ_DESCRIPTION,
                        RequestGroupDescription = request.REF_REQUEST_GROUP_TB.SZ_DESCRIPTION == null ? "" : request.REF_REQUEST_GROUP_TB.SZ_DESCRIPTION,
                        TestGroupDescription = request.REF_TEST_GROUP_TB.SZ_DESCRIPTION == null ? "" : request.REF_TEST_GROUP_TB.SZ_DESCRIPTION
                    };
                    requestList.Add(item);
                }

                return Json(requestList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: RequestController.GetRequests_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: ComplainantController.GetRequests_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
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
                var requestEditViewModel = new RequestEditViewModel();
                if (ID > 0)
                {
                    REF_REQUEST_TB request = _uow.Repository<REF_REQUEST_TB>().GetById(ID);
                    requestEditViewModel.RequestID = request.N_REQUEST_SYSID;
                    requestEditViewModel.SelectedLocationGroup = request.N_LOCATION_GROUP_SYSID;
                    requestEditViewModel.SelectedRequestGroup = request.N_REQUEST_GROUP_SYSID;
                    requestEditViewModel.SelectedTestGroup = request.N_TEST_GROUP_SYSID;
                    requestEditViewModel.LocationGroups = (List<SelectListItem>)_locationRepo.GetLocationGroupsSelectList();
                    requestEditViewModel.RequestGroups = (List<SelectListItem>)_requestRepo.GetRequestGroups();
                    requestEditViewModel.TestGroups = (List<SelectListItem>)_testRepo.GetTestGroupsSelectList();
                }
                else
                {
                    requestEditViewModel.RequestID = ID;
                    requestEditViewModel.SelectedLocationGroup = 0;
                    requestEditViewModel.SelectedRequestGroup = 0;
                    requestEditViewModel.SelectedTestGroup = 0;
                    requestEditViewModel.LocationGroups = (List<SelectListItem>)_locationRepo.GetLocationGroupsSelectList();
                    requestEditViewModel.RequestGroups = (List<SelectListItem>)_requestRepo.GetRequestGroups();
                    requestEditViewModel.TestGroups = (List<SelectListItem>)_testRepo.GetTestGroupsSelectList();
                }

                return View("Edit", requestEditViewModel);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: RequestController.Edit_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: RequestController.Edit_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult Edit(RequestEditViewModel Model, bool IsTest = false)
        {
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    if (ModelState.IsValid)
                    {
                        int id = 0;
                        if (Model.RequestID == 0) //new
                        {
                            string enteredBy = IsTest ? "Unit Test Case" : _modifiedBy;
                            REF_REQUEST_TB request = new REF_REQUEST_TB()
                            {
                                B_INACTIVE = false,
                                DT_ENTERED = DateTime.UtcNow,
                                DT_MODIFIED = DateTime.UtcNow,
                                N_LOCATION_GROUP_SYSID = Model.SelectedLocationGroup,
                                N_REQUEST_GROUP_SYSID = Model.SelectedRequestGroup,
                                N_TEST_GROUP_SYSID = Model.SelectedTestGroup,
                                SZ_ENTERED_BY = enteredBy,
                                SZ_MODIFIED_BY = _modifiedBy
                            };
                            _uow.Repository<REF_REQUEST_TB>().Add(request);
                            _uow.SaveChanges();
                            REF_REQUEST_TB requestFound = _uow.Repository<REF_REQUEST_TB>().Find(u => u.SZ_ENTERED_BY == enteredBy
                                && u.SZ_MODIFIED_BY == _modifiedBy && u.N_LOCATION_GROUP_SYSID == Model.SelectedLocationGroup
                                && u.N_REQUEST_GROUP_SYSID == Model.SelectedRequestGroup && u.N_TEST_GROUP_SYSID == Model.SelectedTestGroup
                                ).FirstOrDefault();
                            id = requestFound.N_REQUEST_SYSID;
                        }
                        else //edit
                        {
                            REF_REQUEST_TB request = _uow.Repository<REF_REQUEST_TB>().GetById(Model.RequestID);
                            request.DT_MODIFIED = DateTime.UtcNow;
                            request.N_LOCATION_GROUP_SYSID = Model.SelectedLocationGroup;
                            request.N_REQUEST_GROUP_SYSID = Model.SelectedRequestGroup;
                            request.N_TEST_GROUP_SYSID = Model.SelectedTestGroup;
                            request.SZ_MODIFIED_BY = _modifiedBy;

                            _uow.Repository<REF_REQUEST_TB>().Update(request);
                            _uow.SaveChanges();
                            id = Model.RequestID;
                        }

                        return RedirectToAction("Index", new { ID = id });
                    }

                    Model.LocationGroups = (List<SelectListItem>)_locationRepo.GetLocationGroupsSelectList();
                    Model.RequestGroups = (List<SelectListItem>)_requestRepo.GetRequestGroups();
                    Model.TestGroups = (List<SelectListItem>)_testRepo.GetTestGroupsSelectList();

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
                        ViewBag.Message = "Function: RequestController.Edit_POST\n\nError: " + ex.Message + "\n\n" + sb;
                    }
                    else
                    {
                        ViewBag.Message = "Function: RequestController.Edit_POST\n\nError: " + ex.Message + "\n\n" + sb + "\n\n" + ex.InnerException.Message;
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
                        ViewBag.Message = "Function: RequestController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.GetBaseException().Message;
                    }
                    else
                    {
                        ViewBag.Message = "Function: RequestController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.GetBaseException().Message + "\n\nInnerException: " + ex.InnerException.Message;
                    };
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        ViewBag.Message = "Function: RequestController.Edit_POST\n\nError: " + ex.Message;
                    }
                    else
                    {
                        ViewBag.Message = "Function: RequestController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.InnerException.Message;
                    };
                };
            } while (saveFailed);

            Session["ErrorMessage"] = ViewBag.Message;
            return RedirectToAction("InternalServerError", "Error");
        }

        [HttpGet]
        public ActionResult Delete(string ID)
        {
            try
            {
                REF_REQUEST_TB request = _uow.Repository<REF_REQUEST_TB>().GetById(Convert.ToInt32(ID));
                RequestDeleteViewModel requestDeleteViewModel = new RequestDeleteViewModel()
                {
                    LocationGroup = _locationRepo.GetLocationGroupDescriptionByID(Convert.ToInt32(request.N_LOCATION_GROUP_SYSID)),
                    Message = "",
                    RequestGroup = _requestRepo.GetRequestGroupDescriptionByRequestGroupID(Convert.ToInt32(request.N_REQUEST_GROUP_SYSID)),
                    RequestID = request.N_REQUEST_SYSID,
                    SelectedLocationGroup = request.N_LOCATION_GROUP_SYSID,
                    SelectedRequestGroup = request.N_REQUEST_GROUP_SYSID,
                    SelectedTestGroup = request.N_TEST_GROUP_SYSID,
                    ShowMessage = false,
                    TestGroup = _testRepo.GetTestGroupDescriptionByID(Convert.ToInt32(request.N_TEST_GROUP_SYSID))
                };

                return View(requestDeleteViewModel);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: RequestController.Delete_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: RequestController.Delete_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult Delete(RequestDeleteViewModel model)
        {
            try
            {
                int id = Convert.ToInt32(model.RequestID);

                _uow.Repository<REF_REQUEST_TB>().Delete(id);
                _uow.SaveChanges();

                return RedirectToAction("Index", new { ID = id });
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: RequestController.Delete_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: RequestController.Delete_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };      
        }
    }
}