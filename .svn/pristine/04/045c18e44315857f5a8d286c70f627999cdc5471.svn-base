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
    public class ReceivingWaterController : BaseController
    {
        private GenericEHWaterQualityUnitOfWork _uow = null;
        private ReceivingWaterGet _receivingWaterRepo = null;

        private string _modifiedBy = null;
        private string _userRole = null;
        //private WebServerClient _webServerClient;
        //private HttpClient _client;
        //private Uri _resourceServerUri = new Uri(ConfigurationManager.AppSettings["OAuth.MacombCounty.Paths.Remote.ResourceServerBaseAddress"]);

        public ReceivingWaterController()
        {
            _uow = new GenericEHWaterQualityUnitOfWork();
            _receivingWaterRepo = new ReceivingWaterGet();

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
                var model = new ReceivingWaterIndexViewModel()
                {
                    ID = ID
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: ReceivingWaterController.Index_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: ReceivingWaterController.Index_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult GetReceivingWaters()
        {
            try
            {
                IEnumerable<REF_WATER_BODY_TB> receivingWaters = _uow.Repository<REF_WATER_BODY_TB>().GetAll();
                List<ReceivingWaterIndex> receivingWaterList = new List<ReceivingWaterIndex>();

                receivingWaters = receivingWaters.ToList().OrderBy(u => u.SZ_DESCRIPTION);

                foreach (var receivingWater in receivingWaters)
                {
                    ReceivingWaterIndex item = new ReceivingWaterIndex()
                    {
                        ReceivingWaterDescription = receivingWater.SZ_DESCRIPTION == null ? "" : receivingWater.SZ_DESCRIPTION,
                        ReceivingWaterID = receivingWater.N_WATER_BODY_SYSID
                    };
                    receivingWaterList.Add(item);
                }

                return Json(receivingWaterList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: ReceivingWaterController.GetReceivingWaters_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: ReceivingWaterController.GetReceivingWaters_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
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
                var receivingWaterEditViewModel = new ReceivingWaterEditViewModel();
                if (ID > 0)
                {
                    REF_WATER_BODY_TB receivingWater = _uow.Repository<REF_WATER_BODY_TB>().GetById(ID);
                    receivingWaterEditViewModel.ReceivingWaterDescription = receivingWater.SZ_DESCRIPTION == null ? "" : receivingWater.SZ_DESCRIPTION;
                    receivingWaterEditViewModel.ReceivingWaterID = receivingWater.N_WATER_BODY_SYSID;
                    receivingWaterEditViewModel.ShowMessageDescription = false;
                    receivingWaterEditViewModel.MessageDescription = "";
                }
                else
                {
                    receivingWaterEditViewModel.ReceivingWaterDescription = "";
                    receivingWaterEditViewModel.ReceivingWaterID = ID;
                    receivingWaterEditViewModel.ShowMessageDescription = false;
                    receivingWaterEditViewModel.MessageDescription = "";
                }

                return View("Edit", receivingWaterEditViewModel);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: ReceivingWaterController.Edit_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: ReceivingWaterController.Edit_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult Edit(ReceivingWaterEditViewModel Model)
        {
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    if (_receivingWaterRepo.IsReceivingWaterDescriptionDuplicate(Model.ReceivingWaterDescription))
                    {
                        Model.ShowMessageDescription = true;
                        Model.MessageDescription = "The Receiving Water Description already exists. Please enter a different one.";

                        return View(Model);
                    }
                    else
                    {
                        if (ModelState.IsValid)
                        {
                            int id = 0;
                            if (Model.ReceivingWaterID == 0) //new
                            {
                                int receivingWaterID = _receivingWaterRepo.GetNextRecWaterID();
                                REF_WATER_BODY_TB receivingWater = new REF_WATER_BODY_TB()
                                {
                                    SZ_ENTERED_BY = _modifiedBy,
                                    DT_ENTERED = DateTime.UtcNow,
                                    SZ_MODIFIED_BY = _modifiedBy,
                                    DT_MODIFIED = DateTime.UtcNow,
                                    SZ_NAME = Model.ReceivingWaterDescription,
                                    SZ_DESCRIPTION = Model.ReceivingWaterDescription
                                };
                                _uow.Repository<REF_WATER_BODY_TB>().Add(receivingWater);
                                _uow.SaveChanges();
                                REF_WATER_BODY_TB receivingWaterFound = _uow.Repository<REF_WATER_BODY_TB>().Find(u => u.SZ_ENTERED_BY == _modifiedBy
                                    && u.SZ_MODIFIED_BY == _modifiedBy && u.SZ_DESCRIPTION == Model.ReceivingWaterDescription
                                    ).FirstOrDefault();
                                id = receivingWaterFound.N_WATER_BODY_SYSID;
                            }
                            else //edit
                            {
                                REF_WATER_BODY_TB receivingWater = _uow.Repository<REF_WATER_BODY_TB>().GetById(Model.ReceivingWaterID);
                                receivingWater.SZ_MODIFIED_BY = _modifiedBy;
                                receivingWater.DT_MODIFIED = DateTime.UtcNow;
                                receivingWater.SZ_DESCRIPTION = Model.ReceivingWaterDescription;

                                _uow.Repository<REF_WATER_BODY_TB>().Update(receivingWater);
                                _uow.SaveChanges();
                                id = Model.ReceivingWaterID;
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
                        ViewBag.Message = "Function: ReceivingWaterController.Edit_POST\n\nError: " + ex.Message + "\n\n" + sb;
                    }
                    else
                    {
                        ViewBag.Message = "Function: ReceivingWaterController.Edit_POST\n\nError: " + ex.Message + "\n\n" + sb + "\n\n" + ex.InnerException.Message;
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
                        ViewBag.Message = "Function: ReceivingWaterController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.GetBaseException().Message;
                    }
                    else
                    {
                        ViewBag.Message = "Function: ReceivingWaterController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.GetBaseException().Message + "\n\nInnerException: " + ex.InnerException.Message;
                    };
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        ViewBag.Message = "Function: ReceivingWaterController.Edit_POST\n\nError: " + ex.Message;
                    }
                    else
                    {
                        ViewBag.Message = "Function: ReceivingWaterController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.InnerException.Message;
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
                REF_WATER_BODY_TB recWater = _uow.Repository<REF_WATER_BODY_TB>().GetById(Convert.ToInt32(ID));
                ReceivingWaterDeleteViewModel recWaterDeleteViewModel = new ReceivingWaterDeleteViewModel()
                {
                    ReceivingWaterID = recWater.N_WATER_BODY_SYSID,
                    ReceivingWaterDescription = recWater.SZ_DESCRIPTION,
                    MessageDescription = "",
                    ShowMessageDescription = false
                };

                return View(recWaterDeleteViewModel);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: ReceivingWaterController.Delete_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: ReceivingWaterController.Delete_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult Delete(ReceivingWaterDeleteViewModel model)
        {
            try
            {
                int id = Convert.ToInt32(model.ReceivingWaterID);

                _uow.Repository<REF_WATER_BODY_TB>().Delete(id);
                _uow.SaveChanges();

                return RedirectToAction("Index", new { ID = id });
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: ReceivingWaterController.Delete_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: ReceivingWaterController.Delete_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };       
        }
    }
}