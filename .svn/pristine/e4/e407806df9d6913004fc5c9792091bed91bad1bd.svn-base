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
    public class TestGroupController : BaseController
    {
        private GenericEHWaterQualityUnitOfWork _uow = null;
        private TestGet _testRepo = null;

        private string _modifiedBy = null;
        private string _userRole = null;
        //private WebServerClient _webServerClient;
        //private HttpClient _client;
        //private Uri _resourceServerUri = new Uri(ConfigurationManager.AppSettings["OAuth.MacombCounty.Paths.Remote.ResourceServerBaseAddress"]);

        public TestGroupController()
        {
            _uow = new GenericEHWaterQualityUnitOfWork();
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
                var model = new TestGroupIndexViewModel()
                {
                    ID = ID
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: TestGroupController.Index_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: TestGroupController.Index_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult GetTestGroups()
        {
            try
            {
                IEnumerable<REF_TEST_GROUP_TB> testGroups = _uow.Repository<REF_TEST_GROUP_TB>().GetAll();
                List<TestGroupIndex> testGroupList = new List<TestGroupIndex>();

                testGroups = testGroups.ToList().OrderBy(u => u.SZ_DESCRIPTION);

                foreach (var testGroup in testGroups)
                {
                    TestGroupIndex item = new TestGroupIndex()
                    {
                        TestGroupDescription = testGroup.SZ_DESCRIPTION == null ? "" : testGroup.SZ_DESCRIPTION,
                        TestGroupID = testGroup.N_TEST_GROUP_SYSID
                    };
                    testGroupList.Add(item);
                }

                return Json(testGroupList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: TestGroupController.GetTestGroups_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: TestGroupController.GetTestGroups_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
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
                var testGroupEditViewModel = new TestGroupEditViewModel();
                if (ID > 0)
                {
                    REF_TEST_GROUP_TB testGroup = _uow.Repository<REF_TEST_GROUP_TB>().GetById(ID);
                    testGroupEditViewModel.TestGroupDescription = testGroup.SZ_DESCRIPTION;
                    testGroupEditViewModel.TestGroupID = testGroup.N_TEST_GROUP_SYSID;
                    testGroupEditViewModel.ShowMessageDescription = false;
                    testGroupEditViewModel.MessageDescription = "";
                }
                else
                {
                    testGroupEditViewModel.TestGroupDescription = "";
                    testGroupEditViewModel.TestGroupID = ID;
                    testGroupEditViewModel.ShowMessageDescription = false;
                    testGroupEditViewModel.MessageDescription = "";
                }

                return View("Edit", testGroupEditViewModel);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: TestGroupController.Edit_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: TestGroupController.Edit_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult Edit(TestGroupEditViewModel Model)
        {
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    if (_testRepo.IsTestGroupDescriptionDuplicate(Model.TestGroupDescription))
                    {
                        Model.ShowMessageDescription = true;
                        Model.MessageDescription = "The Test Group Description already exists. Please enter a different one.";

                        return View(Model);
                    }
                    else
                    {
                        if (ModelState.IsValid)
                        {
                            int id = 0;
                            if (Model.TestGroupID == 0) //new
                            {
                                REF_TEST_GROUP_TB testGroup = new REF_TEST_GROUP_TB()
                                {
                                    B_INACTIVE = false,
                                    DT_ENTERED = DateTime.UtcNow,
                                    DT_MODIFIED = DateTime.UtcNow,
                                    SZ_DESCRIPTION = Model.TestGroupDescription,
                                    SZ_ENTERED_BY = _modifiedBy,
                                    SZ_MODIFIED_BY = _modifiedBy
                                };
                                _uow.Repository<REF_TEST_GROUP_TB>().Add(testGroup);
                                _uow.SaveChanges();
                                REF_TEST_GROUP_TB testGroupFound = _uow.Repository<REF_TEST_GROUP_TB>().Find(u => u.SZ_ENTERED_BY == _modifiedBy
                                    && u.SZ_MODIFIED_BY == _modifiedBy && u.SZ_DESCRIPTION == Model.TestGroupDescription).FirstOrDefault();
                                id = testGroupFound.N_TEST_GROUP_SYSID;
                            }
                            else //edit
                            {
                                REF_TEST_GROUP_TB testGroup = _uow.Repository<REF_TEST_GROUP_TB>().GetById(Model.TestGroupID);
                                testGroup.DT_MODIFIED = DateTime.UtcNow;
                                testGroup.SZ_DESCRIPTION = Model.TestGroupDescription;
                                testGroup.SZ_MODIFIED_BY = _modifiedBy;
                                _uow.Repository<REF_TEST_GROUP_TB>().Update(testGroup);
                                _uow.SaveChanges();
                                id = Model.TestGroupID;
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
                        ViewBag.Message = "Function: TestGroupController.Edit_POST\n\nError: " + ex.Message + "\n\n" + sb;
                    }
                    else
                    {
                        ViewBag.Message = "Function: TestGroupController.Edit_POST\n\nError: " + ex.Message + "\n\n" + sb + "\n\n" + ex.InnerException.Message;
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
                        ViewBag.Message = "Function: TestGroupController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.GetBaseException().Message;
                    }
                    else
                    {
                        ViewBag.Message = "Function: TestGroupController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.GetBaseException().Message + "\n\nInnerException: " + ex.InnerException.Message;
                    };
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        ViewBag.Message = "Function: TestGroupController.Edit_POST\n\nError: " + ex.Message;
                    }
                    else
                    {
                        ViewBag.Message = "Function: TestGroupController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.InnerException.Message;
                    };
                };
            } while (saveFailed);

            Session["ErrorMessage"] = ViewBag.Message;
            return RedirectToAction("InternalServerError", "Error");
        }

        [HttpGet]
        public ActionResult Details(int ID, int? TestID)
        {
            try
            {
                REF_TEST_GROUP_TB testGroup = _uow.Repository<REF_TEST_GROUP_TB>().GetById(ID);

                TestGroupDetailsViewModel model = new TestGroupDetailsViewModel()
                {
                    TestGroupDescription = testGroup.SZ_DESCRIPTION == null ? "" : testGroup.SZ_DESCRIPTION,
                    TestGroupID = ID,
                    TestID = Convert.ToInt32(TestID)
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: TestGroupController.Details_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: TestGroupController.Details_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult GetTests(string TestGroupID)
        {
            try
            {
                int testGroupID = Convert.ToInt32(TestGroupID);
                IEnumerable<REF_TEST_X_TEST_GROUP_TB> testGroupByTest = _testRepo.GetTestsByTestGroupID(testGroupID);
                List<TestDetails> testDetails = new List<TestDetails>();

                foreach (var row in testGroupByTest)
                {
                    TestDetails item = new TestDetails()
                    {
                        TestDescription = row.REF_TEST_TB.SZ_DESCRIPTION,
                        TestGroupID = row.N_TEST_GROUP_SYSID,
                        TestID = row.N_TEST_SYSID
                    };
                    testDetails.Add(item);
                }

                return Json(testDetails, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: TestGroupController.GetTests_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: TestGroupController.GetTests_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult Delete(int ID, string TestID)
        {
            try
            {
                bool isDeleted = _testRepo.DeleteTestByTestGroupByIDs(Convert.ToInt32(TestID), ID);

                return RedirectToAction("Details", new { ID = ID });
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: TestGroupController.Delete_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: TestGroupController.Delete_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult Create(int ID, int? TestID)
        {
            try
            {
                REF_TEST_GROUP_TB testGroup = _uow.Repository<REF_TEST_GROUP_TB>().GetById(ID);

                TestGroupCreateTestViewModel model = new TestGroupCreateTestViewModel()
                {
                    ID = ID,
                    SelectedTest = 0,
                    TestGroupDescription = testGroup.SZ_DESCRIPTION,
                    Tests = (List<SelectListItem>)_testRepo.GetTestsSelectListByTestGroupID(ID)
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: TestGroupController.Create_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: TestGroupController.Create_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult Create(TestGroupCreateTestViewModel Model, bool IsTest = false)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string modifiedBy = IsTest ? "Unit Test Case" : _modifiedBy;
                    REF_TEST_X_TEST_GROUP_TB record = new REF_TEST_X_TEST_GROUP_TB()
                    {
                        B_INACTIVE = false,
                        DT_ENTERED = DateTime.UtcNow,
                        DT_MODIFIED = DateTime.UtcNow,
                        N_TEST_GROUP_SYSID = Model.ID,
                        N_TEST_SYSID = Model.SelectedTest,
                        SZ_ENTERED_BY = modifiedBy,
                        SZ_MODIFIED_BY = _modifiedBy
                    };

                    _uow.Repository<REF_TEST_X_TEST_GROUP_TB>().Add(record);
                    _uow.SaveChanges();

                    return RedirectToAction("Details", new { ID = Model.ID, TestGroupID = Model.SelectedTest });
                }

                Model.Tests = (List<SelectListItem>)_testRepo.GetTestGroupsSelectListByTestID(Model.ID);

                return View(Model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: TestGroupController.Create_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: TestGroupController.Create_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult DeleteTestGr(string ID)
        {
            try
            {
                REF_TEST_GROUP_TB testGroup = _uow.Repository<REF_TEST_GROUP_TB>().GetById(Convert.ToInt32(ID));
                TestGroupDeleteViewModel testGroupDeleteViewModel = new TestGroupDeleteViewModel()
                {
                    TestGroupID = testGroup.N_TEST_GROUP_SYSID,
                    TestGroupDescription = testGroup.SZ_DESCRIPTION,
                    MessageDescription = "",
                    ShowMessageDescription = false
                };

                return View(testGroupDeleteViewModel);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: TestGroupController.DeleteTestGr_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: TestGroupController.DeleteTestGr_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult DeleteTestGr(TestGroupDeleteViewModel model)
        {
            try
            {
                int id = Convert.ToInt32(model.TestGroupID);
                if (_uow.Repository<REF_REQUEST_TB>().Find(u => u.N_TEST_GROUP_SYSID == id).Count() > 0 ||
                    _uow.Repository<REF_TEST_X_TEST_GROUP_TB>().Find(u => u.N_TEST_GROUP_SYSID == id).Count() > 0 ||
                    _uow.Repository<TBL_CHEMISTRY_DATA>().Find(u => u.N_TEST_GROUP_SYSID == id).Count() > 0 ||
                    _uow.Repository<TBL_ECOLI_DATA>().Find(u => u.N_ECOLI_SYSID == id).Count() > 0)
                {
                    model.ShowMessageDescription = true;
                    model.MessageDescription = "Records exist for this test group. Sorry, the test group can not be deleted.";

                    return View(model);
                }
                else
                {
                    _uow.Repository<REF_TEST_GROUP_TB>().Delete(id);
                    _uow.SaveChanges();

                    return RedirectToAction("Index", new { ID = id });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: TestGroupController.DeleteTestGr_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: TestGroupController.DeleteTestGr_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };   
        }
    }
}