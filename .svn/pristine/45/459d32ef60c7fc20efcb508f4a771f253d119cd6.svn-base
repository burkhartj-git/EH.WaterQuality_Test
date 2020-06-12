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
    public class TestController : BaseController
    {
        private GenericEHWaterQualityUnitOfWork _uow = null;
        private TestGet _testRepo = null;

        private string _modifiedBy = null;
        private string _userRole = null;
        //private WebServerClient _webServerClient;
        //private HttpClient _client;
        //private Uri _resourceServerUri = new Uri(ConfigurationManager.AppSettings["OAuth.MacombCounty.Paths.Remote.ResourceServerBaseAddress"]);

        public TestController()
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
                var model = new TestIndexViewModel()
                {
                    ID = ID
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: TestController.Index_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: TestController.Index_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult GetTests()
        {
            try
            {
                IEnumerable<REF_TEST_TB> tests = _uow.Repository<REF_TEST_TB>().GetAll();
                List<TestIndex> testList = new List<TestIndex>();

                tests = tests.ToList().OrderBy(u => u.SZ_DESCRIPTION).ThenBy(u => u.SZ_LABEL);

                foreach (var test in tests)
                {
                    TestIndex item = new TestIndex()
                    {
                        AMethod = test.SZ_ANALYSIS_METHOD == null ? "" : test.SZ_ANALYSIS_METHOD,
                        EffectiveDate = test.DT_EFFECTIVE == null || test.DT_EFFECTIVE.Value.ToShortDateString() == "1/1/0001" ? "" : test.DT_EFFECTIVE.Value.ToShortDateString(),
                        ExpiredDate = test.DT_EXPIRED == null || test.DT_EXPIRED.Value.ToShortDateString() == "1/1/0001" ? "" : test.DT_EXPIRED.Value.ToShortDateString(),
                        SampleMedia = test.SZ_SAMPLE_MEDIA == null ? "" : test.SZ_SAMPLE_MEDIA,
                        SampleType = test.SZ_SAMPLE_TYPE == null ? "" : test.SZ_SAMPLE_TYPE,
                        TestID = test.N_TEST_SYSID,
                        TestName = test.SZ_DESCRIPTION == null ? "" : test.SZ_DESCRIPTION,
                        Units = test.SZ_TITLE == null ? "" : test.SZ_TITLE
                    };
                    testList.Add(item);
                }

                return Json(testList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: TestController.GetTests_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: TestController.GetTests_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
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
                var testEditViewModel = new TestEditViewModel();
                if (ID > 0)
                {
                    REF_TEST_TB test = _uow.Repository<REF_TEST_TB>().GetById(ID);
                    testEditViewModel.AnalysisMethod = test.SZ_ANALYSIS_METHOD;
                    testEditViewModel.Description = test.SZ_DESCRIPTION;
                    testEditViewModel.EffectiveDate = test.DT_EFFECTIVE == null ? "" : test.DT_EFFECTIVE.Value.ToShortDateString();
                    testEditViewModel.ExpiredDate = test.DT_EXPIRED == null ? "" : test.DT_EXPIRED.Value.ToShortDateString();
                    testEditViewModel.SampleMedia = test.SZ_SAMPLE_MEDIA;
                    testEditViewModel.SampleType = test.SZ_SAMPLE_TYPE;
                    testEditViewModel.TestID = test.N_TEST_SYSID;
                    testEditViewModel.Unit = test.SZ_TITLE;
                }
                else
                {
                    testEditViewModel.AnalysisMethod = "";
                    testEditViewModel.Description = "";
                    testEditViewModel.EffectiveDate = "";
                    testEditViewModel.ExpiredDate = "";
                    testEditViewModel.SampleMedia = "";
                    testEditViewModel.SampleType = "";
                    testEditViewModel.TestID = ID;
                    testEditViewModel.Unit = "";
                }

                return View("Edit", testEditViewModel);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: TestController.Edit_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: TestController.Edit_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult Edit(TestEditViewModel Model)
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
                        if (Model.TestID == 0) //new
                        {
                            REF_TEST_TB test = new REF_TEST_TB()
                            {
                                B_INACTIVE = false,
                                DT_EFFECTIVE = Model.EffectiveDate == "" ? (DateTime?)null : Convert.ToDateTime(Model.EffectiveDate),
                                DT_ENTERED = DateTime.UtcNow,
                                DT_EXPIRED = Model.ExpiredDate == "" ? (DateTime?)null : Convert.ToDateTime(Model.ExpiredDate),
                                DT_MODIFIED = DateTime.UtcNow,
                                SZ_DESCRIPTION = Model.Description,
                                SZ_ENTERED_BY = _modifiedBy,
                                SZ_MODIFIED_BY = _modifiedBy,
                                SZ_ANALYSIS_METHOD = Model.AnalysisMethod,
                                SZ_SAMPLE_MEDIA = Model.SampleMedia,
                                SZ_SAMPLE_TYPE = Model.SampleType,
                                SZ_TITLE = Model.Unit
                            };
                            _uow.Repository<REF_TEST_TB>().Add(test);
                            _uow.SaveChanges();
                            DateTime effectiveDate = Convert.ToDateTime(Model.EffectiveDate);
                            DateTime expiredDate = Convert.ToDateTime(Model.ExpiredDate);
                            REF_TEST_TB testFound = _uow.Repository<REF_TEST_TB>().Find(u => u.SZ_ENTERED_BY == _modifiedBy
                                && u.SZ_MODIFIED_BY == _modifiedBy && u.DT_EFFECTIVE == effectiveDate && u.DT_EXPIRED == expiredDate
                                && u.SZ_DESCRIPTION == Model.Description && u.SZ_ANALYSIS_METHOD == Model.AnalysisMethod
                                && u.SZ_SAMPLE_MEDIA == Model.SampleMedia && u.SZ_SAMPLE_TYPE == Model.SampleType
                                && u.SZ_TITLE == Model.Unit).FirstOrDefault();
                            id = testFound.N_TEST_SYSID;
                        }
                        else //edit
                        {
                            REF_TEST_TB test = _uow.Repository<REF_TEST_TB>().GetById(Model.TestID);
                            test.DT_EFFECTIVE = Convert.ToDateTime(Model.EffectiveDate);
                            test.DT_EXPIRED = Convert.ToDateTime(Model.ExpiredDate);
                            test.DT_MODIFIED = DateTime.UtcNow;
                            test.SZ_ANALYSIS_METHOD = Model.AnalysisMethod;
                            test.SZ_DESCRIPTION = Model.Description;
                            test.SZ_MODIFIED_BY = _modifiedBy;
                            test.SZ_SAMPLE_MEDIA = Model.SampleMedia;
                            test.SZ_SAMPLE_TYPE = Model.SampleType;
                            test.SZ_TITLE = Model.Unit;
                            _uow.Repository<REF_TEST_TB>().Update(test);
                            _uow.SaveChanges();
                            id = Model.TestID;
                        }

                        return RedirectToAction("Index", new { ID = id });
                    }

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
                        ViewBag.Message = "Function: TestController.Edit_POST\n\nError: " + ex.Message + "\n\n" + sb;
                    }
                    else
                    {
                        ViewBag.Message = "Function: TestController.Edit_POST\n\nError: " + ex.Message + "\n\n" + sb + "\n\n" + ex.InnerException.Message;
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
                        ViewBag.Message = "Function: TestController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.GetBaseException().Message;
                    }
                    else
                    {
                        ViewBag.Message = "Function: TestController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.GetBaseException().Message + "\n\nInnerException: " + ex.InnerException.Message;
                    };
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        ViewBag.Message = "Function: TestController.Edit_POST\n\nError: " + ex.Message;
                    }
                    else
                    {
                        ViewBag.Message = "Function: TestController.Edit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.InnerException.Message;
                    };
                };
            } while (saveFailed);

            Session["ErrorMessage"] = ViewBag.Message;
            return RedirectToAction("InternalServerError", "Error");
        }

        [HttpGet]
        public ActionResult Details(int ID, int? TestGroupID)
        {
            try
            {
                REF_TEST_TB test = _uow.Repository<REF_TEST_TB>().GetById(ID);

                TestDetailsViewModel model = new TestDetailsViewModel()
                {
                    AnalysisMethod = test.SZ_ANALYSIS_METHOD == null ? "" : test.SZ_ANALYSIS_METHOD,
                    SampleMedia = test.SZ_SAMPLE_MEDIA == null ? "" : test.SZ_SAMPLE_MEDIA,
                    SampleType = test.SZ_SAMPLE_TYPE == null ? "" : test.SZ_SAMPLE_TYPE,
                    ShortDescription = test.SZ_DESCRIPTION == null ? "" : test.SZ_DESCRIPTION,
                    TestGroupID = Convert.ToInt32(TestGroupID),
                    TestID = test.N_TEST_SYSID,
                    Unit = test.SZ_TITLE == null ? "" : test.SZ_TITLE,
                    EffectiveDate = test.DT_EFFECTIVE == null ? "" : test.DT_EFFECTIVE.Value.ToShortDateString(),
                    ExpiredDate = test.DT_EXPIRED == null ? "" : test.DT_EXPIRED.Value.ToShortDateString()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: TestController.Details_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: TestController.Details_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult GetTestGroups(string TestID)
        {
            try
            {
                int testID = Convert.ToInt32(TestID);
                IEnumerable<REF_TEST_X_TEST_GROUP_TB> testByTestGroup = _testRepo.GetTestGroupsByTestID(testID);
                List<TestDetails> testDetails = new List<TestDetails>();

                foreach (var row in testByTestGroup)
                {
                    TestDetails item = new TestDetails()
                    {
                        TestGroupDescription = row.REF_TEST_GROUP_TB.SZ_DESCRIPTION,
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
                    ViewBag.Message = "Function: TestController.GetTestGroups_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: TestController.GetTestGroups_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult Delete(int ID, string TestGroupID)
        {
            try
            {
                bool isDeleted = _testRepo.DeleteTestByTestGroupByIDs(ID, Convert.ToInt32(TestGroupID));

                return RedirectToAction("Details", new { ID = ID });
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: TestController.Delete_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: TestController.Delete_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult Create(int ID, int? TestGroupID)
        {
            try
            {
                REF_TEST_TB test = _uow.Repository<REF_TEST_TB>().GetById(ID);

                TestCreateGroupViewModel model = new TestCreateGroupViewModel()
                {
                    ID = ID,
                    SelectedTestGroup = 0,
                    TestDescription = test.SZ_DESCRIPTION,
                    TestGroups = (List<SelectListItem>)_testRepo.GetTestGroupsSelectListByTestID(ID)
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: TestController.Create_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: TestController.Create_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult Create(TestCreateGroupViewModel Model, bool IsTest = false)
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
                        N_TEST_GROUP_SYSID = Model.SelectedTestGroup,
                        N_TEST_SYSID = Model.ID,
                        SZ_ENTERED_BY = modifiedBy,
                        SZ_MODIFIED_BY = _modifiedBy
                    };

                    _uow.Repository<REF_TEST_X_TEST_GROUP_TB>().Add(record);
                    _uow.SaveChanges();

                    return RedirectToAction("Details", new { ID = Model.ID, TestGroupID = Model.SelectedTestGroup });
                }

                Model.TestGroups = (List<SelectListItem>)_testRepo.GetTestGroupsSelectListByTestID(Model.ID);

                return View(Model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: TestController.Create_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: TestController.Create_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        public ActionResult DeleteTest(string ID)
        {
            try
            {
                REF_TEST_TB test = _uow.Repository<REF_TEST_TB>().GetById(Convert.ToInt32(ID));
                TestDeleteViewModel testDeleteViewModel = new TestDeleteViewModel()
                {
                    AnalysisMethod = test.SZ_ANALYSIS_METHOD,
                    Description = test.SZ_DESCRIPTION,
                    EffectiveDate = test.DT_EFFECTIVE == null ? "" : test.DT_EFFECTIVE.Value.ToShortDateString(),
                    ExpiredDate = test.DT_EXPIRED == null ? "" : test.DT_EXPIRED.Value.ToShortDateString(),
                    SampleMedia = test.SZ_SAMPLE_MEDIA,
                    SampleType = test.SZ_SAMPLE_TYPE,
                    TestID = test.N_TEST_SYSID,
                    Unit = test.SZ_TITLE,
                    Message = "",
                    ShowMessage = false
                };

                return View(testDeleteViewModel);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: TestController.DeleteTest_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: TestController.DeleteTest_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        public ActionResult DeleteTest(TestDeleteViewModel model)
        {
            try
            {
                int id = Convert.ToInt32(model.TestID);
                if (_uow.Repository<REF_TEST_X_TEST_GROUP_TB>().Find(u => u.N_TEST_SYSID == id).Count() > 0 ||
                    _uow.Repository<TRN_RESULT_TB>().Find(u => u.N_TEST_SYSID == id).Count() > 0)
                {
                    model.ShowMessage = true;
                    model.Message = "Records exist for this test. Sorry, the test can not be deleted.";

                    return View(model);
                }
                else
                {
                    _uow.Repository<REF_TEST_TB>().Delete(id);
                    _uow.SaveChanges();

                    return RedirectToAction("Index", new { ID = id });
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: TestController.DeleteTest_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: TestController.DeleteTest_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };    
        }
    }
}