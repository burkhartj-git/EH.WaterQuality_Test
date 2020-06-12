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
    public class RequestGroupController : BaseController
    {
        private GenericEHWaterQualityUnitOfWork _uow = null;
        private RequestGroupGet _requestGroupRepo = null;

        private string _modifiedBy = null;
        private string _userRole = null;
        //private WebServerClient _webServerClient;
        //private HttpClient _client;
        //private Uri _resourceServerUri = new Uri(ConfigurationManager.AppSettings["OAuth.MacombCounty.Paths.Remote.ResourceServerBaseAddress"]);

        public RequestGroupController()
        {
            _uow = new GenericEHWaterQualityUnitOfWork();
            _requestGroupRepo = new RequestGroupGet();

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
                var model = new RequestGroupIndexViewModel()
                {
                    ID = ID
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: RequestGroupController.Index_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: RequestGroupController.Index_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult GetRequestGroups()
        {
            try
            {
                IEnumerable<REF_REQUEST_GROUP_TB> requestGroups = _uow.Repository<REF_REQUEST_GROUP_TB>().GetAll();
                List<RequestGroupIndex> requestGroupList = new List<RequestGroupIndex>();

                requestGroups = requestGroups.ToList().OrderBy(u => u.SZ_DESCRIPTION);

                foreach (var requestGroup in requestGroups)
                {
                    RequestGroupIndex item = new RequestGroupIndex()
                    {
                        RequestGroupID = requestGroup.N_REQUEST_GROUP_SYSID,
                        RequestGroupDescription = requestGroup.SZ_DESCRIPTION == null ? "" : requestGroup.SZ_DESCRIPTION
                    };
                    requestGroupList.Add(item);
                }

                return Json(requestGroupList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: RequestGroupController.GetRequestGroups_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: RequestGroupController.GetRequestGroups_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
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
                var requestGroupEditViewModel = new RequestGroupEditViewModel();
                if (ID > 0)
                {
                    REF_REQUEST_GROUP_TB requestGroup = _uow.Repository<REF_REQUEST_GROUP_TB>().GetById(ID);
                    requestGroupEditViewModel.RequestGroupDescription = requestGroup.SZ_DESCRIPTION == null ? "" : requestGroup.SZ_DESCRIPTION;
                    requestGroupEditViewModel.RequestGroupID = requestGroup.N_REQUEST_GROUP_SYSID;
                    requestGroupEditViewModel.ShowMessageDescription = false;
                    requestGroupEditViewModel.MessageDescription = "";
                }
                else
                {
                    requestGroupEditViewModel.RequestGroupID = ID;
                    requestGroupEditViewModel.RequestGroupDescription = "";
                    requestGroupEditViewModel.ShowMessageDescription = false;
                    requestGroupEditViewModel.MessageDescription = "";
                }

                return View("Edit", requestGroupEditViewModel);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: RequestGroupController.Edit_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: RequestGroupController.Edit_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult Edit(RequestGroupEditViewModel Model)
        {
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    if (_requestGroupRepo.IsRequestGroupDescriptionDuplicate(Model.RequestGroupDescription))
                    {
                        Model.ShowMessageDescription = true;
                        Model.MessageDescription = "The Request Group Description already exists. Please enter a different one.";

                        return View(Model);
                    }
                    else
                    {
                        if (ModelState.IsValid)
                        {
                            int id = 0;
                            if (Model.RequestGroupID == 0) //new
                            {
                                REF_REQUEST_GROUP_TB requestGroup = new REF_REQUEST_GROUP_TB()
                                {
                                    B_INACTIVE = false,
                                    DT_ENTERED = DateTime.UtcNow,
                                    DT_MODIFIED = DateTime.UtcNow,
                                    SZ_ENTERED_BY = _modifiedBy,
                                    SZ_MODIFIED_BY = _modifiedBy,
                                    SZ_DESCRIPTION = Model.RequestGroupDescription
                                };
                                _uow.Repository<REF_REQUEST_GROUP_TB>().Add(requestGroup);
                                _uow.SaveChanges();
                                REF_REQUEST_GROUP_TB requestGroupFound = _uow.Repository<REF_REQUEST_GROUP_TB>().Find(u => u.SZ_ENTERED_BY == _modifiedBy
                                    && u.SZ_MODIFIED_BY == _modifiedBy && u.SZ_DESCRIPTION == Model.RequestGroupDescription
                                    ).FirstOrDefault();
                                id = requestGroupFound.N_REQUEST_GROUP_SYSID;
                            }
                            else //edit
                            {
                                REF_REQUEST_GROUP_TB requestGroup = _uow.Repository<REF_REQUEST_GROUP_TB>().GetById(Model.RequestGroupID);
                                requestGroup.DT_MODIFIED = DateTime.UtcNow;
                                requestGroup.SZ_DESCRIPTION = Model.RequestGroupDescription;
                                requestGroup.SZ_MODIFIED_BY = _modifiedBy;

                                _uow.Repository<REF_REQUEST_GROUP_TB>().Update(requestGroup);
                                _uow.SaveChanges();
                                id = Model.RequestGroupID;
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
                        ViewBag.Message = "Function: RequestGroupController.Edit_POST\n\nError: " + ex.Message + "\n\n" + sb;
                    }
                    else
                    {
                        ViewBag.Message = "Function: RequestGroupController.Edit_POST\n\nError: " + ex.Message + "\n\n" + sb + "\n\n" + ex.InnerException.Message;
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
                        ViewBag.Message = "Function: RequestGroupController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.GetBaseException().Message;
                    }
                    else
                    {
                        ViewBag.Message = "Function: RequestGroupController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.GetBaseException().Message + "\n\nInnerException: " + ex.InnerException.Message;
                    };
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        ViewBag.Message = "Function: RequestGroupController.Edit_POST\n\nError: " + ex.Message;
                    }
                    else
                    {
                        ViewBag.Message = "Function: RequestGroupController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.InnerException.Message;
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
                REF_REQUEST_GROUP_TB requestGroup = _uow.Repository<REF_REQUEST_GROUP_TB>().GetById(Convert.ToInt32(ID));
                RequestGroupDeleteViewModel requestGroupDeleteViewModel = new RequestGroupDeleteViewModel()
                {
                    RequestGroupID = requestGroup.N_REQUEST_GROUP_SYSID,
                    RequestGroupDescription = requestGroup.SZ_DESCRIPTION,
                    MessageDescription = "",
                    ShowMessageDescription = false
                };

                return View(requestGroupDeleteViewModel);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: RequestGroupController.Delete_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: RequestGroupController.Delete_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult Delete(RequestGroupDeleteViewModel model)
        {
            try
            {
                int id = Convert.ToInt32(model.RequestGroupID);
                if (_uow.Repository<REF_REQUEST_TB>().Find(u => u.N_REQUEST_GROUP_SYSID == id).Count() > 0)
                {
                    model.ShowMessageDescription = true;
                    model.MessageDescription = "Request records exist for this request group. Sorry, the request group can not be deleted.";

                    return View(model);
                }
                else
                {
                    _uow.Repository<REF_REQUEST_GROUP_TB>().Delete(id);
                    _uow.SaveChanges();

                    return RedirectToAction("Index", new { ID = id });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: RequestGroupController.Delete_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: RequestGroupController.Delete_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };  
        }
    }
}