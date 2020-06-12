using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
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
    public class UserController : BaseController
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private GenericEHWaterQualityUnitOfWork _uow = null;
        private FacilityGet _facilityRepo = null;
        private RequestGroupGet _requestGroupRepo = null;
        private LocationGet _locationRepo = null;
        private WaterBodyGet _waterRepo = null;
        private ResultLogGet _resultRepo = null;
        private SampleLogGet _sampleRepo = null;
        private SewerOverflowLogGet _sewerOverflowRepo = null;
        private TestGet _testRepo = null;
        private WeatherDataLogGet _weatherDataRepo = null;

        private string _modifiedBy = null;
        private string _userRole = null;
        //private WebServerClient _webServerClient;
        //private HttpClient _client;
        //private Uri _resourceServerUri = new Uri(ConfigurationManager.AppSettings["OAuth.MacombCounty.Paths.Remote.ResourceServerBaseAddress"]);

        public UserController()
        {
            _uow = new GenericEHWaterQualityUnitOfWork();
            _facilityRepo = new FacilityGet();
            _requestGroupRepo = new RequestGroupGet();
            _locationRepo = new LocationGet();
            _waterRepo = new WaterBodyGet();
            _resultRepo = new ResultLogGet();
            _sampleRepo = new SampleLogGet();
            _sewerOverflowRepo = new SewerOverflowLogGet();
            _testRepo = new TestGet();
            _weatherDataRepo = new WeatherDataLogGet();

            if (SessionHelper.UserName == null)
            {
                _modifiedBy = "Test";
            }
            else
            {
                _modifiedBy = SessionHelper.UserName;
            }
        }

        #region - SampleBatch -

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        [OutputCache(Duration = 0)]
        public ActionResult SampleBatch()
        {
            try
            {
                IEnumerable<SelectListItem> requestGroups = _requestGroupRepo.GetRequestGroups();

                var model = new SampleBatchViewModel()
                {
                    RequestGroups = (List<SelectListItem>)requestGroups,
                    CollectedDate = DateTime.UtcNow.ToShortDateString()
                };
                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.SampleBatch_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.SampleBatch_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult GetLocationsByRequestGroupID(string RequestGroupID)
        {
            try
            {
                var locations = _locationRepo.GetLocationsByRequestGroupID(Convert.ToInt32(RequestGroupID));
                return Json(locations, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.GetLocationsByRequestGroupID_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.GetLocationsByRequestGroupID_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult PutBatches(string Json)
        {
            try
            {
                List<Batch> batchList = JsonConvert.DeserializeObject<List<Batch>>(Json);
                string collectedBy = "";
                string collectedDate = "";
                int counter = 0;
                List<Sample> sampleList = new List<Sample>();

                foreach (var batch in batchList)
                {
                    if (counter == 0)
                    {
                        collectedDate = batch.First.ToString();
                        collectedBy = batch.Second.ToString();
                    }
                    if (counter > 0)
                    {
                        int batchCounter = 1;
                        int batchCount = 1;

                        string numberBatches = batch.Second.ToString();
                        int value;
                        if (int.TryParse(numberBatches, out value))
                        {
                            batchCount = value;
                        }

                        for (int i = 1; i < batchCount + 1; i++)
                        {
                            Sample sample = new Sample()
                            {
                                Batch = batchCounter.ToString(),
                                LocationID = batch.First.ToString(),
                                Location = _locationRepo.GetLocationDescriptionByLocationID(Convert.ToInt32(batch.First)),
                                RequestGroupID = batch.Third.ToString(),
                                RequestGroup = _requestGroupRepo.GetRequestGroupDescriptionByRequestGroupID(Convert.ToInt32(batch.Third)),
                                Label = _locationRepo.GetLocationLabelByLocationID(Convert.ToInt32(batch.First))
                            };
                            sampleList.Add(sample);
                            batchCounter++;
                        }
                    }
                    counter++;
                }

                var model = new SampleViewModel()
                {
                    CollectedBy = collectedBy,
                    CollectedDate = collectedDate
                };

                SampleList.Samples = sampleList;
                TempData["SampleViewModelItem"] = model;
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.PutBatches_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.PutBatches_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        #endregion

        #region - Sample -

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult Sample()
        {
            try
            {
                SampleViewModel model = (SampleViewModel)TempData["SampleViewModelItem"];
                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.Sample_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.Sample_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult GetSampleLocations()
        {
            try
            {
                var locations = SampleList.Samples;
                return Json(locations, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.GetSampleLocations_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.GetSampleLocations_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult PutSamples(string Json)
        {
            //need to refactor code to validate for duplicate entries of composite key in TRN_SAMPLE_TB location date and time
            try
            {
                List<JsonSampleItem> sampleList = JsonConvert.DeserializeObject<List<JsonSampleItem>>(Json);
                string collectedBy = "";
                string collectedDate = "";
                int counter = 0;
                int sampleLogID = 0;
                int[,] sampleLogIDArray = new int[sampleList.Count() - 1, 2];

                foreach (var sample in sampleList)
                {
                    if (counter == 0)
                    {
                        collectedDate = sample.First.ToString();
                        collectedBy = sample.Second.ToString();
                    }
                    if (counter > 0)
                    {
                        sampleLogID = _sampleRepo.GetNextSampleLogID();
                        TRN_SAMPLE_TB sampleNew = new TRN_SAMPLE_TB()
                        {
                            N_SAMPLE_SYSID = sampleLogID,
                            B_INACTIVE = false,
                            DT_COLLECTED = Convert.ToDateTime(collectedDate),
                            DT_ENTERED = DateTime.UtcNow,
                            DT_MODIFIED = DateTime.UtcNow,
                            SZ_BATCH_NUMBER = sample.Second.ToString(),
                            N_COLLECTED_TIME = sample.Third.ToString(),
                            N_LOCATION_SYSID = Convert.ToInt32(sample.First),
                            SZ_COLLECTED_BY = collectedBy,
                            SZ_ENTERED_BY = _modifiedBy,
                            SZ_MODIFIED_BY = _modifiedBy,
                            N_REQUEST_GROUP_SYSID = Convert.ToInt32(sample.Fourth)
                        };
                        _uow.Repository<TRN_SAMPLE_TB>().Add(sampleNew);
                        _uow.SaveChanges();
                        sampleLogIDArray[counter - 1, 0] = sampleLogID;
                        sampleLogIDArray[counter - 1, 1] = Convert.ToInt32(sample.Fourth);
                    }
                    counter++;
                }

                var model = new ResultNewViewModel()
                {
                    CollectedBy = collectedBy,
                    CollectedDate = collectedDate
                };

                SampleList.SampleLogIDs = sampleLogIDArray;
                TempData["ResultNewViewModelItem"] = model;
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.PutSamples_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.PutSamples_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        #endregion

        #region - Result New -

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult ResultNew()
        {
            try
            {
                ResultNewViewModel model = (ResultNewViewModel)TempData["ResultNewViewModelItem"];
                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.ResultNew_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.ResultNew_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult GetResults()
        {
            try
            {
                var sampleLogIDs = SampleList.SampleLogIDs;

                List<Result> results = new List<Result>();

                for (int i = 0; i < sampleLogIDs.GetLength(0); i++)
                {
                    TRN_SAMPLE_TB sample = _sampleRepo.GetSampleLogByID(sampleLogIDs[i, 0]);
                    IEnumerable<Test> tests = _testRepo.GetTestsByRequestGroupID(sampleLogIDs[i, 1]);
                    foreach (var test in tests)
                    {
                        Result result = new Result()
                        {
                            SampleLogID = sampleLogIDs[i, 0].ToString(),
                            Batch = sample.SZ_BATCH_NUMBER,
                            CollectedTime = sample.N_COLLECTED_TIME,
                            LocationID = sample.N_LOCATION_SYSID.ToString(),
                            Location = _locationRepo.GetLocationDescriptionByLocationID(sample.N_LOCATION_SYSID),
                            TestName = test.Description,
                            TestNameID = test.ID,
                            LocationLabel = _locationRepo.GetLocationLabelByLocationID(sample.N_LOCATION_SYSID)
                        };
                        results.Add(result);
                    }
                }

                return Json(results, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.GetResults_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.GetResults_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult PutResults(string Json)
        {
            try
            {
                List<JsonResultItem> resultList = JsonConvert.DeserializeObject<List<JsonResultItem>>(Json);
                string collectedBy = "";
                string collectedDate = "";
                string requestGroup = "";
                int requestGroupID = 0;
                int counter = 0;
                int resultLogID = 0;
                List<int> resultLogIDList = new List<int>();

                foreach (var result in resultList)
                {
                    if (counter == 0)
                    {
                        collectedDate = result.First.ToString();
                        collectedBy = result.Second.ToString();
                    }
                    if (counter > 0)
                    {
                        double number;
                        resultLogID = _resultRepo.GetNextResultLogID();
                        TRN_RESULT_TB resultNew = new TRN_RESULT_TB()
                        {
                            B_INACTIVE = false,
                            DT_ENTERED = DateTime.UtcNow,
                            DT_MODIFIED = DateTime.UtcNow,
                            N_RESULT_SYSID = resultLogID,
                            N_RESULT_VALUE = double.TryParse(result.Fourth, out number) ? Convert.ToDecimal(result.Fourth) : 0,
                            N_SAMPLE_SYSID = Convert.ToInt32(result.Fifth),
                            N_TEST_SYSID = Convert.ToInt32(result.First),
                            SZ_ENTERED_BY = _modifiedBy,
                            SZ_RESULT_VALUE_INDICATOR = result.Second.ToString(),
                            SZ_MODIFIED_BY = _modifiedBy,
                            SZ_TEMPERATURE = result.Third.ToString()
                        };
                        _uow.Repository<TRN_RESULT_TB>().Add(resultNew);
                        _uow.SaveChanges();
                        resultLogIDList.Add(resultLogID);
                    }
                    counter++;
                }

                var model = new ResultConfirmViewModel()
                {
                    CollectedBy = collectedBy,
                    CollectedDate = collectedDate
                };

                TempData["ResultConfirmViewModelItem"] = model;

                return new EmptyResult();
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.PutResults_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.PutResults_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult ResultConfirm()
        {
            try
            {
                ResultConfirmViewModel model = (ResultConfirmViewModel)TempData["ResultConfirmViewModelItem"];
                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.ResultConfirm_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.ResultConfirm_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        #endregion

        #region - Sample Search -

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        [OutputCache(Duration = 0)]
        public ActionResult SampleSearch(string RequestGroupID = "0", string CollectedDate = "", string SearchClicked = "false")
        {
            try
            {
                string date;

                if (CollectedDate == "")
                {
                    date = DateTime.UtcNow.ToString("MM/dd/yyyy");
                }
                else
                {
                    date = Convert.ToDateTime(CollectedDate).ToString("MM/dd/yyyy");
                }

                IEnumerable<SelectListItem> requestGroups = _requestGroupRepo.GetRequestGroups();

                var model = new SampleSearchViewModel()
                {
                    RequestGroups = (List<SelectListItem>)requestGroups,
                    SelectedRequestGroup = Convert.ToInt32(RequestGroupID),
                    CollectedDate = date,
                    SearchClicked = Convert.ToBoolean(SearchClicked)
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.SampleSearch_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.SampleSearch_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult GetSamplesByRequestGroupIDAndDate(string RequestGroupID, string CollectedDate)
        {
            try
            {
                int requestGroupID = Convert.ToInt32(RequestGroupID);
                DateTime collectedDate = Convert.ToDateTime(CollectedDate);

                IEnumerable<TRN_SAMPLE_TB> samples = _sampleRepo.GetSampleLogsByRequestGroupAndCollectedDate(requestGroupID, collectedDate);
                List<SampleSearch> sampleSearches = new List<SampleSearch>();
                foreach (var sample in samples)
                {
                    SampleSearch sampleSearch = new SampleSearch()
                    {
                        BatchNumber = sample.SZ_BATCH_NUMBER,
                        CollectedBy = sample.SZ_COLLECTED_BY,
                        CollectedDate = sample.DT_COLLECTED.ToShortDateString(),
                        CollectedTime = sample.N_COLLECTED_TIME,
                        LocationDescription = _locationRepo.GetLocationDescriptionByLocationID(sample.N_LOCATION_SYSID),
                        LocationID = sample.N_LOCATION_SYSID.ToString(),
                        SampleID = sample.N_SAMPLE_SYSID.ToString(),
                        LocationLabel = _locationRepo.GetLocationLabelByLocationID(sample.N_LOCATION_SYSID)
                    };
                    sampleSearches.Add(sampleSearch);
                }
                return Json(sampleSearches, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.GetSamplesByRequestGroupIDAndDate_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.GetSamplesByRequestGroupIDAndDate_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        [MacAuthorize]
        public ActionResult GetSamplesByDateWithoutRequestGroup(string CollectedDate)
        {
            try
            {
                DateTime collectedDate = Convert.ToDateTime(CollectedDate);

                IEnumerable<TRN_SAMPLE_TB> samples = _sampleRepo.GetSampleLogsByCollectedDateWhenRequestGroupIsNull(collectedDate);
                List<SampleSearch> sampleSearches = new List<SampleSearch>();
                foreach (var sample in samples)
                {
                    SampleSearch sampleSearch = new SampleSearch()
                    {
                        BatchNumber = sample.SZ_BATCH_NUMBER,
                        CollectedBy = sample.SZ_COLLECTED_BY,
                        CollectedDate = sample.DT_COLLECTED.ToShortDateString(),
                        CollectedTime = sample.N_COLLECTED_TIME,
                        LocationDescription = _locationRepo.GetLocationDescriptionByLocationID(sample.N_LOCATION_SYSID),
                        LocationID = sample.N_LOCATION_SYSID.ToString(),
                        SampleID = sample.N_SAMPLE_SYSID.ToString(),
                        LocationLabel = _locationRepo.GetLocationLabelByLocationID(sample.N_LOCATION_SYSID)
                    };
                    sampleSearches.Add(sampleSearch);
                }
                return Json(sampleSearches, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.GetSamplesByDateWithoutRequestGroup_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.GetSamplesByDateWithoutRequestGroup_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult PutSearchSamples(string Json)
        {
            try
            {
                List<SampleJson> sampleList = JsonConvert.DeserializeObject<List<SampleJson>>(Json);
                SampleList.SampleLogIDs = new int[sampleList.Count, 2];
                int counter = 0;
                foreach (var sample in sampleList)
                {
                    SampleList.SampleLogIDs[counter, 0] = Convert.ToInt32(sample.First);
                    counter++;
                }

                return new EmptyResult();
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.PutSearchSamples_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.PutSearchSamples_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult DeleteSearchSamples(string Json)
        {
            try
            {
                List<SampleJson> sampleList = JsonConvert.DeserializeObject<List<SampleJson>>(Json);

                foreach (var sample in sampleList)
                {
                    int sampleID = Convert.ToInt32(sample.First);

                    var resultList = _uow.Repository<TRN_RESULT_TB>().Find(u => u.N_SAMPLE_SYSID == sampleID);

                    foreach (var result in resultList)
                    {
                        int resultID = Convert.ToInt32(result.N_RESULT_SYSID);
                        _uow.Repository<TRN_RESULT_TB>().Delete(resultID);
                    }

                    _uow.Repository<TRN_SAMPLE_TB>().Delete(sampleID);
                    _uow.SaveChanges();
                }

                return new EmptyResult();
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.DeleteSearchSamples_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.DeleteSearchSamples_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        [OutputCache(Duration = 0)]
        public ActionResult EditRequestGroup(string SampleID, string RequestGroupID, string CollectedDate)
        {
            try
            {
                IEnumerable<SelectListItem> requestGroups = _requestGroupRepo.GetRequestGroups();

                TRN_SAMPLE_TB sample = _sampleRepo.GetSampleLogByID(Convert.ToInt32(SampleID));

                string locationID = _locationRepo.GetLocationLabelByLocationID(sample.N_LOCATION_SYSID);

                string locationDescription = _locationRepo.GetLocationDescriptionByLocationID(sample.N_LOCATION_SYSID);

                EditRequestGroupViewModel model = new EditRequestGroupViewModel()
                {
                    BatchNumber = sample.SZ_BATCH_NUMBER,
                    CollectedBy = sample.SZ_COLLECTED_BY,
                    CollectedDate = CollectedDate,
                    CollectedTime = sample.N_COLLECTED_TIME.ToString(),
                    LocationDescription = locationDescription,
                    RequestGroupID = RequestGroupID,
                    RequestGroups = (List<SelectListItem>)requestGroups,
                    SampleID = SampleID,
                    SelectedRequestGroup = 0,
                    SiteID = locationID
                    //SiteID = sample.N_LOCATION_SYSID.ToString()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.EditRequestGroup_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.EditRequestGroup_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult EditRequestGroup(EditRequestGroupViewModel Model)
        {
            try
            {
                if (Model.SelectedRequestGroup != 0)
                {
                    TRN_SAMPLE_TB sample = _sampleRepo.GetSampleLogByID(Convert.ToInt32(Model.SampleID));

                    sample.N_REQUEST_GROUP_SYSID = Model.SelectedRequestGroup;

                    _uow.Repository<TRN_SAMPLE_TB>().Update(sample);
                    _uow.SaveChanges();
                }

                return RedirectToAction("SampleSearch", "User", new { RequestGroupID = Model.RequestGroupID, CollectedDate = Model.CollectedDate, SearchClicked = "true" });
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.EditRequestGroup_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.EditRequestGroup_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        #endregion

        #region - Sample Edit -

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        [OutputCache(Duration = 0)]
        public ActionResult SampleEdit(string RequestGroupID, string CollectedDate)
        {
            try
            {
                SampleEditViewModel model = new SampleEditViewModel
                {
                    CollectedDate = CollectedDate,
                    RequestGroupID = RequestGroupID
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.SampleEdit_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.SampleEdit_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        [OutputCache(Duration = 0)]
        public ActionResult SampleEditConfirm(string RequestGroupID, string CollectedDate)
        {
            try
            {
                SampleEditConfirmViewModel model = new SampleEditConfirmViewModel
                {
                    CollectedDate = CollectedDate,
                    RequestGroupID = RequestGroupID
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.SampleEditConfirm_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.SampleEditConfirm_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult GetSamplesForEdit()
        {
            try
            {
                IEnumerable<TRN_SAMPLE_TB> samples = _sampleRepo.GetSampleLogsBySampleIDs();
                List<SampleSearch> sampleEdits = new List<SampleSearch>();
                foreach (var sample in samples)
                {
                    SampleSearch sampleEdit = new SampleSearch()
                    {
                        BatchNumber = sample.SZ_BATCH_NUMBER,
                        CollectedBy = sample.SZ_COLLECTED_BY,
                        CollectedDate = sample.DT_COLLECTED.ToShortDateString(),
                        CollectedTime = sample.N_COLLECTED_TIME,
                        LocationDescription = _locationRepo.GetLocationDescriptionByLocationID(sample.N_LOCATION_SYSID),
                        LocationID = sample.N_LOCATION_SYSID.ToString(),
                        SampleID = sample.N_SAMPLE_SYSID.ToString(),
                        LocationLabel = _locationRepo.GetLocationLabelByLocationID(sample.N_LOCATION_SYSID)
                    };
                    sampleEdits.Add(sampleEdit);
                }
                return Json(sampleEdits, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.GetSamplesForEdit_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.GetSamplesForEdit_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult PutSampleEdits(string Json)
        {
            try
            {
                List<JsonSampleItem> sampleList = JsonConvert.DeserializeObject<List<JsonSampleItem>>(Json);

                foreach (var sample in sampleList)
                {
                    TRN_SAMPLE_TB sampleLog = _uow.Repository<TRN_SAMPLE_TB>().GetById(Convert.ToInt32(sample.First));
                    sampleLog.SZ_BATCH_NUMBER = sample.Second;
                    sampleLog.N_COLLECTED_TIME = sample.Third;
                    sampleLog.DT_MODIFIED = DateTime.UtcNow;
                    sampleLog.SZ_MODIFIED_BY = _modifiedBy;
                    _uow.Repository<TRN_SAMPLE_TB>().Update(sampleLog);
                    _uow.SaveChanges();
                }

                return new EmptyResult();
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.PutSampleEdits_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.PutSampleEdits_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        #endregion

        #region - Result Edit -

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        [OutputCache(Duration = 0)]
        public ActionResult ResultEdit(string RequestGroupID, string CollectedDate)
        {
            try
            {
                ResultEditViewModel model = new ResultEditViewModel
                {
                    CollectedDate = CollectedDate,
                    RequestGroupID = RequestGroupID
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.ResultEdit_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.ResultEdit_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        [OutputCache(Duration = 0)]
        public ActionResult ResultEditConfirm(string RequestGroupID, string CollectedDate)
        {
            try
            {
                ResultEditConfirmViewModel model = new ResultEditConfirmViewModel
                {
                    CollectedDate = CollectedDate,
                    RequestGroupID = RequestGroupID
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.ResultEditConfirm_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.ResultEditConfirm_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        [OutputCache(Duration = 0)]
        public ActionResult ResultDeleteConfirm()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.ResultDeleteConfirm_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.ResultDeleteConfirm_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult GetResultsForEdit()
        {
            try
            {
                IEnumerable<TRN_RESULT_TB> results = _resultRepo.GetResultLogsBySampleIDs();
                List<ResultEdit> resultEdits = new List<ResultEdit>();
                foreach (var result in results)
                {
                    ResultEdit resultEdit = new ResultEdit()
                    {
                        BatchNumber = result.TRN_SAMPLE_TB.SZ_BATCH_NUMBER,
                        CollectDate = result.TRN_SAMPLE_TB.DT_COLLECTED.ToShortDateString(),
                        CollectedBy = result.TRN_SAMPLE_TB.SZ_COLLECTED_BY,
                        CollectTime = result.TRN_SAMPLE_TB.N_COLLECTED_TIME.ToString(),
                        Flag = result.SZ_RESULT_VALUE_INDICATOR,
                        LocationDescription = _locationRepo.GetLocationDescriptionByLocationID(result.TRN_SAMPLE_TB.N_LOCATION_SYSID),
                        LocationID = result.TRN_SAMPLE_TB.N_LOCATION_SYSID.ToString(),
                        ReportedResult = result.N_RESULT_VALUE.ToString(),
                        ResultLogID = result.N_RESULT_SYSID.ToString(),
                        SampleLogID = result.N_SAMPLE_SYSID.ToString(),
                        Temperature = result.SZ_TEMPERATURE,
                        TestName = _testRepo.GetTestDescriptionByTestID(result.N_TEST_SYSID),
                        LocationLabel = _locationRepo.GetLocationLabelByLocationID(result.TRN_SAMPLE_TB.N_LOCATION_SYSID)
                    };
                    resultEdits.Add(resultEdit);
                }
                return Json(resultEdits, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.GetResultsForEdit_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.GetResultsForEdit_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult PutResultEdits(string Json)
        {
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    List<JsonResultItem> resultList = JsonConvert.DeserializeObject<List<JsonResultItem>>(Json);

                    foreach (var result in resultList)
                    {
                        TRN_RESULT_TB resultLog = _uow.Repository<TRN_RESULT_TB>().GetById(Convert.ToInt32(result.First));
                        resultLog.SZ_RESULT_VALUE_INDICATOR = result.Second;
                        resultLog.SZ_TEMPERATURE = result.Third;
                        resultLog.N_RESULT_VALUE = Convert.ToDecimal(result.Fourth);
                        resultLog.DT_MODIFIED = DateTime.UtcNow;
                        resultLog.SZ_MODIFIED_BY = _modifiedBy;
                        _uow.Repository<TRN_RESULT_TB>().Update(resultLog);
                        _uow.SaveChanges();
                    }

                    return new EmptyResult();
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
                        ViewBag.Message = "Function: UserController.PutResultEdits_POST\n\nError: " + ex.Message + "\n\n" + sb;
                    }
                    else
                    {
                        ViewBag.Message = "Function: UserController.PutResultEdits_POST\n\nError: " + ex.Message + "\n\n" + sb + "\n\n" + ex.InnerException.Message;
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
                        ViewBag.Message = "Function: UserController.PutResultEdits_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.GetBaseException().Message;
                    }
                    else
                    {
                        ViewBag.Message = "Function: UserController.PutResultEdits_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.GetBaseException().Message + "\n\nInnerException: " + ex.InnerException.Message;
                    };
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        ViewBag.Message = "Function: UserController.PutResultEdits_POST\n\nError: " + ex.Message;
                    }
                    else
                    {
                        ViewBag.Message = "Function: UserController.PutResultEdits_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.InnerException.Message;
                    };
                };
            } while (saveFailed);

            Session["ErrorMessage"] = ViewBag.Message;
            return RedirectToAction("InternalServerError", "Error");
        }

        [HttpPost]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult DeleteResultEdits(string Json)
        {
            try
            {
                List<JsonResultItem> resultList = JsonConvert.DeserializeObject<List<JsonResultItem>>(Json);

                foreach (var result in resultList)
                {
                    _uow.Repository<TRN_RESULT_TB>().Delete(Convert.ToInt32(result.First));
                    _uow.SaveChanges();
                }

                return new EmptyResult();
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.DeleteResultEdits_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.DeleteResultEdits_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        #endregion

        #region - Sewer Overflow Activity -

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        [OutputCache(Duration = 0)]
        public ActionResult SewerOverflowActivity(string FacilityID = "0", string SearchStartDate = "", string SearchEndDate = "", string SearchClicked = "false")
        {
            try
            {
                string startDate;
                string endDate;

                if (SearchStartDate == "")
                {
                    startDate = DateTime.UtcNow.ToShortDateString();
                }
                else
                {
                    startDate = SearchStartDate;
                }

                if (SearchEndDate == "")
                {
                    endDate = DateTime.UtcNow.ToShortDateString();
                }
                else
                {
                    endDate = SearchEndDate;
                }

                IEnumerable<SelectListItem> facilities = _facilityRepo.GetFacilities();
                SewerOverflowActivityViewModel model = new SewerOverflowActivityViewModel()
                {
                    Facilities = (List<SelectListItem>)facilities,
                    SelectedFacility = Convert.ToInt32(FacilityID),
                    StartDate = startDate,
                    EndDate = endDate,
                    SearchClicked = SearchClicked
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.SewerOverflowActivity_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.SewerOverflowActivity_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult GetSewerOverflowActivities(string Facility, string StartDate, string EndDate)
        {
            try
            {
                IEnumerable<TRN_SEWER_OVERFLOW_TB> activities =
                    _sewerOverflowRepo.GetSewerOverflowActivityLogsByFacilityAndDates(Convert.ToInt32(Facility),
                                                                StartDate, EndDate);
                List<SewerOverflowActivitySearch> activitySearches = new List<SewerOverflowActivitySearch>();
                foreach (var activity in activities)
                {
                    SewerOverflowActivitySearch activitySearch = new SewerOverflowActivitySearch()
                    {
                        ActivityType = activity.SZ_ACTIVITY_TYPE == null || activity.SZ_ACTIVITY_TYPE == "0" ? "" : activity.SZ_ACTIVITY_TYPE,
                        Chlorinate = activity.SZ_CHLORINATE == null || activity.SZ_CHLORINATE == "0" ? "" : activity.SZ_CHLORINATE,
                        DischargeGallons = activity.N_DISCHARGE_GALLONS == null ? "" : activity.N_DISCHARGE_GALLONS.ToString(),
                        Duration = activity.N_DURATION_MINUTES == null ? "" : activity.N_DURATION_MINUTES.ToString(),
                        EndDate = activity.DT_ACTIVITY_END == null ? "" : activity.DT_ACTIVITY_END.Value.ToShortDateString(),
                        EndTime = activity.SZ_ACTIVITY_END_TIME == null ? "" : activity.SZ_ACTIVITY_END_TIME,
                        NPDES = activity.SZ_NPDESCOMP == null || activity.SZ_NPDESCOMP == "0" ? "" : activity.SZ_NPDESCOMP,
                        ReceivingWater = activity.REF_WATER_BODY_TB.SZ_DESCRIPTION == null ? "" : activity.REF_WATER_BODY_TB.SZ_DESCRIPTION,
                        SewerLogID = activity.N_SEWER_OVERFLOW_SYSID.ToString(),
                        StartDate = activity.DT_ACTIVITY_START == null ? "" : activity.DT_ACTIVITY_START.Value.ToShortDateString(),
                        StartTime = activity.SZ_ACTIVITY_START_TIME == null ? "" : activity.SZ_ACTIVITY_START_TIME,
                        Title = activity.REF_FACILITY_TB.SZ_TITLE == null ? "" : activity.REF_FACILITY_TB.SZ_TITLE,
                        Precipitation = activity.N_PRECIPITATION == null ? "" : activity.N_PRECIPITATION.ToString()
                    };
                    activitySearches.Add(activitySearch);
                }

                return Json(activitySearches, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.GetSewerOverflowActivities_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.GetSewerOverflowActivities_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult PutSewerOverflowActivity(string Json)
        {
            try
            {
                List<SewerOverflowActivityEdit> sewerLogIDs = JsonConvert.DeserializeObject<List<SewerOverflowActivityEdit>>(Json);

                int logID = Convert.ToInt32(sewerLogIDs.FirstOrDefault().SewerLogID);

                TempData["SewerLogID"] = logID;

                return new EmptyResult();
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.PutSewerOverflowActivity_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.PutSewerOverflowActivity_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        #endregion

        #region - Sewer Overflow Activity Edit -

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        [OutputCache(Duration = 0)]
        public ActionResult SewerOverflowActivityEdit(string FacilityID, string SearchStartDate, string SearchEndDate)
        {
            try
            {
                int logID = Convert.ToInt32(TempData["SewerLogID"]);
                TempData["SewerLogID"] = null;

                IEnumerable<SelectListItem> facilities = _facilityRepo.GetFacilities();
                IEnumerable<SelectListItem> waters = _waterRepo.GetWaterBodies();
                List<SelectListItem> chlorinates = GetChlorinateOptions();
                List<SelectListItem> npdes_s = GetNPDESOptions();
                List<SelectListItem> activityTypes = GetActivityTypeOptions();

                if (logID == 0)
                {
                    SewerOverflowActivityEditViewModel model = new SewerOverflowActivityEditViewModel()
                    {
                        ActivityTypes = activityTypes,
                        Chlorinates = chlorinates,
                        Facilities = (List<SelectListItem>)facilities,
                        NPDESs = npdes_s,
                        ReceivingWaters = (List<SelectListItem>)waters,
                        SelectedReceivingWater = 0,
                        SewerLogID = 0,
                        FacilityID = FacilityID,
                        SearchStartDate = SearchStartDate,
                        SearchEndDate = SearchEndDate
                    };
                    return View(model);
                }
                else
                {
                    TRN_SEWER_OVERFLOW_TB activity = _uow.Repository<TRN_SEWER_OVERFLOW_TB>().GetById(logID);
                    string startTime = activity.SZ_ACTIVITY_START_TIME;
                    string endTime = activity.SZ_ACTIVITY_END_TIME;
                    SewerOverflowActivityEditViewModel model = new SewerOverflowActivityEditViewModel()
                    {
                        ActivityTypes = activityTypes,
                        Chlorinates = chlorinates,
                        Facilities = (List<SelectListItem>)facilities,
                        NPDESs = npdes_s,
                        ReceivingWaters = (List<SelectListItem>)waters,
                        DischargeGallons = activity.N_DISCHARGE_GALLONS == null ? "" : activity.N_DISCHARGE_GALLONS.ToString(),
                        Duration = activity.N_DURATION_MINUTES == null ? "" : activity.N_DURATION_MINUTES.ToString(),
                        EndDate = activity.DT_ACTIVITY_END == null ? "01/01/1900" : activity.DT_ACTIVITY_END.Value.ToString("MM/dd/yyyy"),
                        EndTime = endTime == null ? "" : (endTime.Length < 4 ? "0" + endTime : endTime),
                        SelectedActivityType = activity.SZ_ACTIVITY_TYPE,
                        SelectedChlorinate = activity.SZ_CHLORINATE,
                        SelectedFacility = Convert.ToInt32(activity.N_FACILITY_SYSID),
                        SelectedNPDES = activity.SZ_NPDESCOMP,
                        SelectedReceivingWater = Convert.ToInt32(activity.N_WATER_BODY_SYSID),
                        SewerLogID = activity.N_SEWER_OVERFLOW_SYSID,
                        StartDate = activity.DT_ACTIVITY_START.Value.ToString("MM/dd/yyyy"),
                        StartTime = startTime == null ? "" : (startTime.Length < 4 ? "0" + startTime : startTime),
                        Precipitation = activity.N_PRECIPITATION == null ? "" : activity.N_PRECIPITATION.ToString(),
                        FacilityID = FacilityID,
                        SearchStartDate = SearchStartDate,
                        SearchEndDate = SearchEndDate
                    };
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.SewerOverflowActivityEdit_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.SewerOverflowActivityEdit_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult SewerOverflowActivityEdit(SewerOverflowActivityEditViewModel Model)
        {
            bool saveFailed;
            do
            {
                saveFailed = false;
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (Model.SewerLogID == 0)
                        {
                            DateTime dtEnd;
                            DateTime dtStart;
                            TRN_SEWER_OVERFLOW_TB activity = new TRN_SEWER_OVERFLOW_TB()
                            {
                                B_INACTIVE = false,
                                DT_ACTIVITY_END = Model.EndDate == null ? (DateTime?)null : (DateTime.TryParse(Model.EndDate, out dtEnd) ? dtEnd : (DateTime?)null),
                                DT_ACTIVITY_START = DateTime.TryParse(Model.StartDate, out dtStart) ? dtStart : (DateTime?)null,
                                DT_MODIFIED = DateTime.UtcNow,
                                DT_ENTERED = DateTime.UtcNow,
                                N_DISCHARGE_GALLONS = Model.DischargeGallons == null ? 0 : Convert.ToDecimal(Model.DischargeGallons),
                                N_DURATION_MINUTES = Model.Duration == null ? 0 : Convert.ToDecimal(Model.Duration),
                                N_FACILITY_SYSID = Model.SelectedFacility,
                                N_WATER_BODY_SYSID = Model.SelectedReceivingWater,
                                SZ_ACTIVITY_END_TIME = Model.EndTime == null ? "" : Model.EndTime.ToString(),
                                SZ_ACTIVITY_START_TIME = Model.StartTime == null ? "" : Model.StartTime.ToString(),
                                SZ_ACTIVITY_TYPE = Model.SelectedActivityType == null ? "0" : Model.SelectedActivityType,
                                SZ_CHLORINATE = Model.SelectedChlorinate == null ? "0" : Model.SelectedChlorinate,
                                SZ_MODIFIED_BY = _modifiedBy,
                                SZ_NPDESCOMP = Model.SelectedNPDES == null ? "0" : Model.SelectedNPDES,
                                SZ_ENTERED_BY = _modifiedBy,
                                N_PRECIPITATION = Model.Precipitation == null ? 0 : Convert.ToDouble(Model.Precipitation)
                            };
                            _uow.Repository<TRN_SEWER_OVERFLOW_TB>().Add(activity);
                            _uow.SaveChanges();
                        }
                        else
                        {
                            DateTime dtEnd;
                            TRN_SEWER_OVERFLOW_TB activity = _uow.Repository<TRN_SEWER_OVERFLOW_TB>().GetById(Model.SewerLogID);
                            activity.DT_ACTIVITY_END = Model.EndDate == null ? (DateTime?)null : (DateTime.TryParse(Model.EndDate, out dtEnd) ? dtEnd : (DateTime?)null);
                            activity.DT_ACTIVITY_START = Convert.ToDateTime(Model.StartDate);
                            activity.DT_MODIFIED = DateTime.UtcNow;
                            activity.N_DISCHARGE_GALLONS = Model.DischargeGallons == null ? 0 : Convert.ToDecimal(Model.DischargeGallons);
                            activity.N_DURATION_MINUTES = Model.Duration == null ? 0 : Convert.ToDecimal(Model.Duration);
                            activity.N_FACILITY_SYSID = Model.SelectedFacility;
                            activity.N_WATER_BODY_SYSID = Model.SelectedReceivingWater;
                            activity.SZ_ACTIVITY_END_TIME = Model.EndTime == null ? "" : Model.EndTime.ToString();
                            activity.SZ_ACTIVITY_START_TIME = Model.StartTime == null ? "" : Model.StartTime.ToString();
                            activity.SZ_ACTIVITY_TYPE = Model.SelectedActivityType == null ? "0" : Model.SelectedActivityType;
                            activity.SZ_CHLORINATE = Model.SelectedChlorinate == null ? "0" : Model.SelectedChlorinate;
                            activity.SZ_MODIFIED_BY = _modifiedBy;
                            activity.SZ_NPDESCOMP = Model.SelectedNPDES == null ? "0" : Model.SelectedNPDES;
                            activity.N_PRECIPITATION = Model.Precipitation == null ? 0 : Convert.ToDouble(Model.Precipitation);
                            _uow.Repository<TRN_SEWER_OVERFLOW_TB>().Update(activity);
                            _uow.SaveChanges();
                        }
                        return RedirectToAction("SewerOverflowActivityEditConfirm", new { FacilityID = Model.FacilityID, StartDate = Model.SearchStartDate, EndDate = Model.SearchEndDate });
                    }

                    IEnumerable<SelectListItem> facilities = _facilityRepo.GetFacilities();
                    IEnumerable<SelectListItem> waters = _waterRepo.GetWaterBodies();
                    List<SelectListItem> chlorinates = GetChlorinateOptions();
                    List<SelectListItem> npdes_s = GetNPDESOptions();
                    List<SelectListItem> activityTypes = GetActivityTypeOptions();

                    Model.ActivityTypes = activityTypes;
                    Model.Chlorinates = chlorinates;
                    Model.Facilities = (List<SelectListItem>)facilities;
                    Model.NPDESs = npdes_s;
                    Model.ReceivingWaters = (List<SelectListItem>)waters;

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
                        ViewBag.Message = "Function: UserController.SewerOverflowActivityEdit_POST\n\nError: " + ex.Message + "\n\n" + sb;
                    }
                    else
                    {
                        ViewBag.Message = "Function: UserController.SewerOverflowActivityEdit_POST\n\nError: " + ex.Message + "\n\n" + sb + "\n\n" + ex.InnerException.Message;
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
                        ViewBag.Message = "Function: UserController.SewerOverflowActivityEdit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.GetBaseException().Message;
                    }
                    else
                    {
                        ViewBag.Message = "Function: UserController.SewerOverflowActivityEdit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.GetBaseException().Message + "\n\nInnerException: " + ex.InnerException.Message;
                    };
                }
                catch (Exception ex)
                {
                    if (ex.InnerException == null)
                    {
                        ViewBag.Message = "Function: UserController.SewerOverflowActivityEdit_POST\n\nError: " + ex.Message;
                    }
                    else
                    {
                        ViewBag.Message = "Function: UserController.SewerOverflowActivityEdit_POST\n\nError: " + ex.Message + "\n\nBaseException: " + ex.InnerException.Message;
                    };
                };
            } while (saveFailed);

            Session["ErrorMessage"] = ViewBag.Message;
            return RedirectToAction("InternalServerError", "Error");
        }

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult SewerOverflowActivityEditConfirm(string FacilityID, string StartDate, string EndDate)
        {
            try
            {
                SewerOverflowActivityEditConfirmViewModel model = new SewerOverflowActivityEditConfirmViewModel()
                {
                    FacilityID = FacilityID,
                    SearchStartDate = StartDate,
                    SearchEndDate = EndDate
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.SewerOverflowActivityEditConfirm_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.SewerOverflowActivityEditConfirm_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult SewerOverflowActivityDelete(string FacilityID, string SearchStartDate, string SearchEndDate, string SewerID)
        {
            try
            {
                _uow.Repository<TRN_SEWER_OVERFLOW_TB>().Delete(Convert.ToInt32(SewerID));
                _uow.SaveChanges();

                return RedirectToAction("SewerOverflowActivity", new { FacilityID = FacilityID, SearchStartDate = SearchStartDate, SearchEndDate = SearchEndDate, SearchClicked = "true" });
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.SewerOverflowActivityDelete_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.SewerOverflowActivityDelete_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        #endregion

        #region - Upload Weather Data -

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult WeatherDataUploadHelper()
        {
            try
            {
                WeatherDataUploadViewModel model = new WeatherDataUploadViewModel()
                {
                    DisplayMessage = false,
                    Message = ""
                };

                return RedirectToAction("WeatherDataUpload", model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.WeatherDataUploadHelper_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.WeatherDataUploadHelper_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult WeatherDataUpload(WeatherDataUploadViewModel Model)
        {
            try
            {
                return View(Model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.WeatherDataUpload_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.WeatherDataUpload_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult WeatherDataUploadPost(WeatherDataUploadViewModel Model)
        {
            try
            {
                if (Request != null)
                {
                    //HttpPostedFileBase file = Request.Files["fileUpload"];

                    //string name = file.FileName.ToString();
                    string name = Model.File.FileName.ToString();
                    string[] parts = name.Split('.');

                    if ((Model.File != null) && (Model.File.ContentLength > 0) && !string.IsNullOrEmpty(Model.File.FileName) && (parts[1] == "xlsx" || parts[1] == "xlsm"))
                    {
                        //string fileName = Path.GetFileName(file.FileName);
                        //string path = Path.Combine(Server.MapPath("~/Content"), fileName).ToString();
                        //file.SaveAs(path);

                        byte[] uploadedFile = new byte[Model.File.InputStream.Length];
                        Model.File.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                        string fileName = Path.GetFileName(Model.File.FileName);
                        FileStream fs = System.IO.File.Create(Server.MapPath(@"~\Content\" + fileName), uploadedFile.Length, FileOptions.None);
                        BinaryWriter bw = new BinaryWriter(fs);
                        bw.Write(uploadedFile);
                        bw.Close();
                        fs.Close();
                        string path = Path.Combine(Server.MapPath(@"~/Content"), fileName).ToString();

                        List<WeatherData> weatherDatas = WeatherExcelLinq.GetExcelWeatherData(path);

                        if (weatherDatas != null)
                        {
                            WeatherDataPreviewViewModel model = new WeatherDataPreviewViewModel();

                            foreach (var weatherData in weatherDatas)
                            {
                                WeatherDataPreviewEditorViewModel preview = new WeatherDataPreviewEditorViewModel()
                                {
                                    Day = weatherData.Month + "/" + weatherData.Day + "/" + weatherData.Year,
                                    MaxTemp = weatherData.TempMax,
                                    MaxWindSpeed = weatherData.MaxWindSpeed,
                                    MaxWindSpeedDir = weatherData.MaxWindDirection,
                                    MeanTemp = weatherData.TempMean,
                                    MinTemp = weatherData.TempMin,
                                    WaterPrecip = weatherData.Precipitation24HrWaterEquiv,
                                };
                                model.WeatherDataPreviews.Add(preview);
                                model.Month = weatherData.Month;
                                model.Year = weatherData.Year;
                            }

                            model.HasErrors = WeatherExcelLinq.ExcelDataHasErrors;
                            model.ErrorMessage = "";
                            model.ExcelFilePath = path;

                            System.IO.File.Delete(path);

                            TempData["weatherDataPreview"] = model;

                            return RedirectToAction("WeatherDataPreview");
                        }
                        else
                        {
                            WeatherDataPreviewViewModel model = new WeatherDataPreviewViewModel();
                            model.WeatherDataPreviews = null;
                            model.Month = "";
                            model.Year = "";
                            model.HasErrors = true;
                            model.ErrorMessage = ExcelFileInfo.ExcelFileError;
                            model.ExcelFilePath = "";

                            TempData["weatherDataPreview"] = model;

                            return RedirectToAction("WeatherDataPreview");
                        }
                    }
                }

                WeatherDataUploadViewModel vm = new WeatherDataUploadViewModel()
                {
                    DisplayMessage = true,
                    Message = "The file for upload is not in the proper file format (Excel 2007 or greater with file extension .xlsx or .xlsm)."
                };

                return RedirectToAction("WeatherDataUpload", vm);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.WeatherDataUpload_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.WeatherDataUpload_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        #endregion

        #region - Preview Weather Data -

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult WeatherDataPreview()
        {
            try
            {
                WeatherDataPreviewViewModel model = (WeatherDataPreviewViewModel)TempData["weatherDataPreview"];

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.WeatherDataPreview_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.WeatherDataPreview_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult WeatherDataPreview(WeatherDataPreviewViewModel Model)
        {
            try
            {
                int selfridgeLocationID = _weatherDataRepo.GetSelfridgeLocationID();

                List<WeatherDataPreviewEditorViewModel> weatherList = Model.WeatherDataPreviews.ToList();

                for (int i = weatherList.Count - 1; i >= 0; i--)
                //foreach (WeatherDataPreviewEditorViewModel preview in Model.WeatherDataPreviews.Reverse<WeatherDataPreviewEditorViewModel>())
                {
                    SessionHelper.Counter = i.ToString();

                    string[] parts = weatherList[i].Day.ToString().Split('/');
                    string month = parts[0].ToUpper();
                    string day = parts[1];
                    string year = parts[2];
                    string monthN = "";

                    switch (month)
                    {
                        case "JAN":
                            monthN = "1";
                            break;
                        case "FEB":
                            monthN = "2";
                            break;
                        case "MAR":
                            monthN = "3";
                            break;
                        case "APR":
                            monthN = "4";
                            break;
                        case "MAY":
                            monthN = "5";
                            break;
                        case "JUN":
                            monthN = "6";
                            break;
                        case "JUL":
                            monthN = "7";
                            break;
                        case "AUG":
                            monthN = "8";
                            break;
                        case "SEP":
                            monthN = "9";
                            break;
                        case "OCT":
                            monthN = "10";
                            break;
                        case "NOV":
                            monthN = "11";
                            break;
                        case "DEC":
                            monthN = "12";
                            break;
                        default:
                            break;
                    }

                    DateTime date = new DateTime(Convert.ToInt32(year), Convert.ToInt32(monthN), Convert.ToInt32(day));

                    TRN_SAMPLE_TB sampleFound = _weatherDataRepo.GetSampleLogByLocationIDAndCollectedDate(selfridgeLocationID, date);

                    if (sampleFound == null)
                    {
                        TRN_SAMPLE_TB sample = new TRN_SAMPLE_TB()
                        {
                            B_INACTIVE = false,
                            DT_COLLECTED = date,
                            DT_MODIFIED = DateTime.UtcNow,
                            N_LOCATION_SYSID = selfridgeLocationID,
                            SZ_BATCH_NUMBER = "1",
                            SZ_COLLECTED_BY = "Selfridge",
                            SZ_MODIFIED_BY = _modifiedBy,
                            SZ_ENTERED_BY = _modifiedBy,
                            DT_ENTERED = DateTime.UtcNow,
                            DT_REQUEST = DateTime.UtcNow
                        };
                        _uow.Repository<TRN_SAMPLE_TB>().Add(sample);
                        _uow.SaveChanges();

                        TRN_SAMPLE_TB sampleNew = _weatherDataRepo.GetSampleLogByLocationIDAndCollectedDate(selfridgeLocationID, date);

                        weatherList[i].SampleID = sampleNew.N_SAMPLE_SYSID.ToString();

                        TRN_RESULT_TB resultMaxTemp = new TRN_RESULT_TB()
                        {
                            B_INACTIVE = false,
                            DT_ENTERED = DateTime.UtcNow,
                            DT_MODIFIED = DateTime.UtcNow,
                            N_RESULT_VALUE = weatherList[i].MaxTemp == "N" ? default(decimal?) : Convert.ToDecimal(weatherList[i].MaxTemp),
                            N_TEST_SYSID = 12,
                            N_SAMPLE_SYSID = Convert.ToInt32(weatherList[i].SampleID),
                            SZ_MODIFIED_BY = _modifiedBy,
                            SZ_ENTERED_BY = _modifiedBy
                        };
                        _uow.Repository<TRN_RESULT_TB>().Add(resultMaxTemp);

                        TRN_RESULT_TB resultMinTemp = new TRN_RESULT_TB()
                        {
                            B_INACTIVE = false,
                            DT_ENTERED = DateTime.UtcNow,
                            DT_MODIFIED = DateTime.UtcNow,
                            N_RESULT_VALUE = weatherList[i].MinTemp == "N" ? default(decimal?) : Convert.ToDecimal(weatherList[i].MinTemp),
                            N_TEST_SYSID = 13,
                            N_SAMPLE_SYSID = Convert.ToInt32(weatherList[i].SampleID),
                            SZ_MODIFIED_BY = _modifiedBy,
                            SZ_ENTERED_BY = _modifiedBy
                        };
                        _uow.Repository<TRN_RESULT_TB>().Add(resultMinTemp);

                        TRN_RESULT_TB resultMeanTemp = new TRN_RESULT_TB()
                        {
                            B_INACTIVE = false,
                            DT_ENTERED = DateTime.UtcNow,
                            DT_MODIFIED = DateTime.UtcNow,
                            N_RESULT_VALUE = weatherList[i].MeanTemp == "N" ? default(decimal?) : Convert.ToDecimal(weatherList[i].MeanTemp),
                            N_TEST_SYSID = 14,
                            N_SAMPLE_SYSID = Convert.ToInt32(weatherList[i].SampleID),
                            SZ_MODIFIED_BY = _modifiedBy,
                            SZ_ENTERED_BY = _modifiedBy
                        };
                        _uow.Repository<TRN_RESULT_TB>().Add(resultMeanTemp);

                        TRN_RESULT_TB resultPrecip = new TRN_RESULT_TB()
                        {
                            B_INACTIVE = false,
                            DT_ENTERED = DateTime.UtcNow,
                            DT_MODIFIED = DateTime.UtcNow,
                            N_RESULT_VALUE = weatherList[i].WaterPrecip == "N" ? default(decimal?) : weatherList[i].WaterPrecip == "T" ? Convert.ToDecimal(0.00001) : Convert.ToDecimal(weatherList[i].WaterPrecip),
                            N_TEST_SYSID = 15,
                            N_SAMPLE_SYSID = Convert.ToInt32(weatherList[i].SampleID),
                            SZ_MODIFIED_BY = _modifiedBy,
                            SZ_ENTERED_BY = _modifiedBy
                        };
                        _uow.Repository<TRN_RESULT_TB>().Add(resultPrecip);

                        TRN_RESULT_TB resultWindSpeed = new TRN_RESULT_TB()
                        {
                            B_INACTIVE = false,
                            DT_ENTERED = DateTime.UtcNow,
                            DT_MODIFIED = DateTime.UtcNow,
                            N_RESULT_VALUE = weatherList[i].MaxWindSpeed == "N" ? default(decimal?) : Convert.ToDecimal(weatherList[i].MaxWindSpeed),
                            N_TEST_SYSID = 16,
                            N_SAMPLE_SYSID = Convert.ToInt32(weatherList[i].SampleID),
                            SZ_MODIFIED_BY = _modifiedBy,
                            SZ_ENTERED_BY = _modifiedBy
                        };
                        _uow.Repository<TRN_RESULT_TB>().Add(resultWindSpeed);

                        TRN_RESULT_TB resultWindDir = new TRN_RESULT_TB()
                        {
                            B_INACTIVE = false,
                            DT_ENTERED = DateTime.UtcNow,
                            DT_MODIFIED = DateTime.UtcNow,
                            N_RESULT_VALUE = weatherList[i].MaxWindSpeedDir == "N" ? default(decimal?) : Convert.ToDecimal(weatherList[i].MaxWindSpeedDir),
                            N_TEST_SYSID = 17,
                            N_SAMPLE_SYSID = Convert.ToInt32(weatherList[i].SampleID),
                            SZ_MODIFIED_BY = _modifiedBy,
                            SZ_ENTERED_BY = _modifiedBy
                        };
                        _uow.Repository<TRN_RESULT_TB>().Add(resultWindDir);

                        _uow.SaveChanges();

                        weatherList.RemoveAt(i);

                        TempData["weatherDataPreviewConfirm"] = "";
                    }
                    else
                    {
                        Model.WeatherDataPreviews = new List<WeatherDataPreviewEditorViewModel>();

                        foreach (var listItem in weatherList)
                        {
                            Model.WeatherDataPreviews.Add(listItem);
                        }

                        // sample record exists
                        TempData["weatherDataPreviewConfirm"] = Model;
                    }
                }

                return RedirectToAction("WeatherDataPreviewConfirm");
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.WeatherDataPreview_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.WeatherDataPreview_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult WeatherDataPreviewConfirm()
        {
            try
            {
                WeatherDataPreviewViewModel model;

                if (TempData["weatherDataPreviewConfirm"].ToString() != "")
                {
                    model = (WeatherDataPreviewViewModel)TempData["weatherDataPreviewConfirm"];
                    model.HasErrors = true;
                }
                else
                {
                    model = new WeatherDataPreviewViewModel()
                    {
                        HasErrors = false
                    };
                }

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.WeatherDataPreviewConfirm_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.WeatherDataPreviewConfirm_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        #endregion

        #region - Search (View) Weather Data -

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        [OutputCache(Duration = 0)]
        public ActionResult WeatherDataSearch()
        {
            try
            {
                WeatherDataSearchViewModel model = new WeatherDataSearchViewModel();

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.WeatherDataSearch_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.WeatherDataSearch_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult GetWeatherDataByMonthAndYear(string Month, string Year)
        {
            try
            {
                IEnumerable<WeatherData> weatherDatas = _weatherDataRepo.GetWeatherDataLogsByMonthAndYear(Month, Year);
                List<WeatherDataSearch> weatherDataSearches = new List<WeatherDataSearch>();
                foreach (var weatherData in weatherDatas)
                {
                    WeatherDataSearch weatherDataSearch = new WeatherDataSearch()
                    {
                        CollectedDate = weatherData.DateCollected,
                        MaxTemp = weatherData.TempMax == "" ? "N" : double.Parse(weatherData.TempMax).ToString(),
                        MeanTemp = weatherData.TempMean == "" ? "N" : double.Parse(weatherData.TempMean).ToString(),
                        MinTemp = weatherData.TempMin == "" ? "N" : double.Parse(weatherData.TempMin).ToString(),
                        WaterPrecip = weatherData.Precipitation24HrWaterEquiv == "T" ? "T" : weatherData.Precipitation24HrWaterEquiv == "" ? "N" : double.Parse(weatherData.Precipitation24HrWaterEquiv).ToString(),
                        WeatherDataID = weatherData.SampleID,
                        WindDirection = weatherData.MaxWindDirection == "" ? "N" : double.Parse(weatherData.MaxWindDirection).ToString(),
                        WindSpeed = weatherData.MaxWindSpeed == "" ? "N" : double.Parse(weatherData.MaxWindSpeed).ToString()
                    };
                    weatherDataSearches.Add(weatherDataSearch);
                }

                return Json(weatherDataSearches, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.GetWeatherDataByMonthAndYear_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.GetWeatherDataByMonthAndYear_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        [MacAuthorize(Action = "ViewPublished")]
        public JsonResult DeleteWeatherData(string JsonString)
        {
            try
            {
                List<JsonSampleItem> weatherDatas = JsonConvert.DeserializeObject<List<JsonSampleItem>>(JsonString);

                foreach (var weatherData in weatherDatas)
                {
                    int sampleID = Convert.ToInt32(weatherData.First);
                    IEnumerable<TRN_RESULT_TB> results = (IEnumerable<TRN_RESULT_TB>)_uow.Repository<TRN_RESULT_TB>().Find(u => u.N_SAMPLE_SYSID == sampleID);

                    foreach (var result in results)
                    {
                        _uow.Repository<TRN_RESULT_TB>().Delete(result);
                    }

                    TRN_SAMPLE_TB sample = _uow.Repository<TRN_SAMPLE_TB>().Find(u => u.N_SAMPLE_SYSID == sampleID).FirstOrDefault();
                    _uow.Repository<TRN_SAMPLE_TB>().Delete(sample);

                    _uow.SaveChanges();
                }

                List<Test> tests = new List<Test>();

                Test test = new Test()
                {
                    ID = "1",
                    Description = "Test"
                };

                tests.Add(test);

                //return new EmptyResult();
                return Json(tests, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.DeleteWeatherData_POST\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.DeleteWeatherData_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                //return RedirectToAction("InternalServerError", "Error");
                return null; //???
            };
        }

        #endregion

        #region - Chemistry Data Upload -

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult ChemistryDataUpload()
        {
            try
            {
                ChemistryDataUploadViewModel model = new ChemistryDataUploadViewModel()
                {
                    ChemistryTestTypes = _testRepo.GetTestDescriptions().ToList()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.ChemistryDataUpload_GET\n\nError: " + ex.Message;
                }
                else
                {
                    ViewBag.Message = "Function: UserController.ChemistryDataUpload_GET\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        [HttpPost]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult ChemistryDataUpload(ChemistryDataUploadViewModel Model)
        {
            try
            {
                if (Request != null)
                {
                    string name = Model.File.FileName.ToString();
                    string[] parts = name.Split('.');

                    if ((Model.File != null) && (Model.File.ContentLength > 0) && !string.IsNullOrEmpty(Model.File.FileName) && (parts[parts.GetUpperBound(0)] == "xlsx" || parts[parts.GetUpperBound(0)] == "xlsm"))
                    {
                        byte[] uploadedFile = new byte[Model.File.InputStream.Length];
                        Model.File.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                        string fileName = Path.GetFileName(Model.File.FileName);
                        FileStream fs = System.IO.File.Create(Server.MapPath(@"~\Content\" + fileName), uploadedFile.Length, FileOptions.None);
                        BinaryWriter bw = new BinaryWriter(fs);
                        bw.Write(uploadedFile);
                        bw.Close();
                        fs.Close();
                        string path = Path.Combine(Server.MapPath(@"~/Content"), fileName).ToString();

                        List<ChemistryData> chemistryDatas = ChemistryExcelLinq.GetExcelChemistryData(path);

                        if (chemistryDatas != null)
                        {

                            //check the notebook on desk - you need to check for existing entry in the sample and associated result table for each chemistryData and if dup[licates found then return
                            //the error message like in the else statement below. then you have to do the same type thing for the ecoli excel upload code - use this as the template.

                            ChemistryDataPreviewViewModel model = new ChemistryDataPreviewViewModel();

                            foreach (var chemistryData in chemistryDatas)
                            {
                                ChemistryDataPreviewEditorViewModel preview = new ChemistryDataPreviewEditorViewModel()
                                {
                                    CollectedDate = chemistryData.CollectedDate,
                                    CollectedTime = chemistryData.CollectedTime,
                                    ResultValue = chemistryData.ReportedResult,
                                    SiteID = chemistryData.SiteID,
                                    TestName = chemistryData.TestName,
                                    AMethod = chemistryData.AMethod,
                                    Flag = chemistryData.Flag,
                                    SampleMedia = chemistryData.SampleMedia,
                                    SampleType = chemistryData.SampleType,
                                    Units = chemistryData.Units
                                };
                                model.ChemistryDataPreviews.Add(preview);
                            }

                            model.HasErrors = ChemistryExcelLinq.ExcelDataHasErrors;
                            model.ErrorMessage = "";
                            model.ExcelFilePath = path;
                            model.SelectedTestType = Model.SelectedChemistryTestType;

                            TempData["chemistryDataPreview"] = model;

                            return RedirectToAction("ChemistryDataPreview");
                        }
                        else
                        {
                            ChemistryDataPreviewViewModel model = new ChemistryDataPreviewViewModel();
                            model.ChemistryDataPreviews = null;
                            model.HasErrors = true;
                            model.ErrorMessage = ExcelFileInfo.ExcelFileError;
                            model.ExcelFilePath = "";
                            model.SelectedTestType = Model.SelectedChemistryTestType;

                            TempData["chemistryDataPreview"] = model;

                            return RedirectToAction("chemistryDataPreview");
                        }
                    }
                }

                Session["ErrorMessage"] = "The file for upload is not in the proper file format (Excel 2007 or greater with file extension .xlsx or .xlsm).";
                return RedirectToAction("InternalServerError", "Error");
            }
            catch (Exception ex)
            {
                if (ex.InnerException == null)
                {
                    ViewBag.Message = "Function: UserController.ChemistryDataUpload_POST\n\nError: " + (ex.Message + "\n\nStack Trace: " + ex.StackTrace);
                }
                else
                {
                    ViewBag.Message = "Function: UserController.ChemistryDataUpload_POST\n\nError: " + (ex.Message + "\n\nInnerException: " + ex.InnerException.Message);
                };
                Session["ErrorMessage"] = ViewBag.Message;
                return RedirectToAction("InternalServerError", "Error");
            };
        }

        #endregion

        #region - Chemistry Data Preview -

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult ChemistryDataPreview()
        {
            ChemistryDataPreviewViewModel model = (ChemistryDataPreviewViewModel)TempData["chemistryDataPreview"];

            return View(model);
        }

        [HttpPost]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult ChemistryDataPreview(ChemistryDataPreviewViewModel Model)
        {
            int batchSampleID = 0;

            List<ChemistryDataPreviewEditorViewModel> chemistryList = Model.ChemistryDataPreviews.ToList();

            for (int i = chemistryList.Count - 1; i >= 0; i--)
            //foreach (ChemistryDataPreviewEditorViewModel preview in Model.ChemistryDataPreviews.Reverse<ChemistryDataPreviewEditorViewModel>())
            {
                string siteID = chemistryList[i].SiteID.ToString();

                int locationID = _uow.Repository<REF_LOCATION_TB>().Find(u => u.SZ_LABEL == siteID).Select(u => u.N_LOCATION_SYSID).FirstOrDefault();

                if (locationID == 0)
                {
                    REF_LOCATION_TB location = new REF_LOCATION_TB()
                    {
                        B_INACTIVE = false,
                        DT_EFFECTIVE = DateTime.UtcNow,
                        DT_ENTERED = DateTime.UtcNow,
                        DT_EXPIRED = DateTime.UtcNow,
                        DT_MODIFIED = DateTime.UtcNow,
                        SZ_DESCRIPTION = "Unknown",
                        SZ_ENTERED_BY = _modifiedBy,
                        SZ_LABEL = chemistryList[i].SiteID,
                        SZ_MODIFIED_BY = _modifiedBy
                    };

                    _uow.Repository<REF_LOCATION_TB>().Add(location);
                    _uow.SaveChanges();

                    locationID = _uow.Repository<REF_LOCATION_TB>().Find(u => u.SZ_LABEL == chemistryList[i].SiteID).Select(u => u.N_LOCATION_SYSID).FirstOrDefault();
                }

                DateTime collectedDate = Convert.ToDateTime(chemistryList[i].CollectedDate);
                string collectedTime = chemistryList[i].CollectedTime.Replace(":00", "").Replace(":", "");

                int sampleID = _uow.Repository<TRN_SAMPLE_TB>().Find(u => u.N_LOCATION_SYSID == locationID && DbFunctions.TruncateTime(u.DT_COLLECTED) == DbFunctions.TruncateTime(collectedDate) && u.N_COLLECTED_TIME == collectedTime).Select(u => u.N_SAMPLE_SYSID).FirstOrDefault();

                if (sampleID == 0)
                {
                    TRN_SAMPLE_TB sample = new TRN_SAMPLE_TB()
                    {
                        B_INACTIVE = false,
                        DT_COLLECTED = collectedDate,
                        DT_ENTERED = DateTime.UtcNow,
                        DT_MODIFIED = DateTime.UtcNow,
                        DT_REQUEST = DateTime.UtcNow,
                        N_COLLECTED_TIME = collectedTime,
                        N_LOCATION_SYSID = locationID,
                        SZ_BATCH_NUMBER = "1",
                        SZ_COLLECTED_BY = "Not Entered",
                        SZ_ENTERED_BY = _modifiedBy,
                        SZ_MODIFIED_BY = _modifiedBy
                    };

                    _uow.Repository<TRN_SAMPLE_TB>().Add(sample);
                    _uow.SaveChanges();

                    batchSampleID = _uow.Repository<TRN_SAMPLE_TB>().Find(u => u.N_LOCATION_SYSID == locationID && DbFunctions.TruncateTime(u.DT_COLLECTED) == DbFunctions.TruncateTime(collectedDate) && u.N_COLLECTED_TIME == collectedTime).Select(u => u.N_SAMPLE_SYSID).FirstOrDefault();
                }

                string testName = chemistryList[i].TestName.ToString();

                int testID = _uow.Repository<REF_TEST_TB>().Find(u => u.SZ_DESCRIPTION == testName).Select(u => u.N_TEST_SYSID).FirstOrDefault();

                if (testID == 0)
                {
                    REF_TEST_TB test = new REF_TEST_TB()
                    {
                        B_INACTIVE = false,
                        DT_EFFECTIVE = DateTime.UtcNow,
                        DT_ENTERED = DateTime.UtcNow,
                        DT_EXPIRED = DateTime.UtcNow,
                        DT_MODIFIED = DateTime.UtcNow,
                        DT_REQUEST = DateTime.UtcNow,
                        SZ_ANALYSIS_METHOD = chemistryList[i].AMethod,
                        SZ_DESCRIPTION = chemistryList[i].TestName,
                        SZ_ENTERED_BY = _modifiedBy,
                        SZ_LABEL = chemistryList[i].SiteID,
                        SZ_MODIFIED_BY = _modifiedBy,
                        SZ_SAMPLE_MEDIA = chemistryList[i].SampleMedia,
                        SZ_SAMPLE_TYPE = chemistryList[i].SampleType,
                        SZ_TITLE = chemistryList[i].Units
                    };

                    _uow.Repository<REF_TEST_TB>().Add(test);
                    _uow.SaveChanges();
                }

                testID = _uow.Repository<REF_TEST_TB>().Find(u => u.SZ_DESCRIPTION == testName).Select(u => u.N_TEST_SYSID).FirstOrDefault();

                int resultID = _uow.Repository<TRN_RESULT_TB>().Find(u => u.N_SAMPLE_SYSID == batchSampleID && u.N_TEST_SYSID == testID).Select(u => u.N_RESULT_SYSID).FirstOrDefault();

                if (resultID == 0)
                {
                    TRN_RESULT_TB result = new TRN_RESULT_TB()
                    {
                        B_INACTIVE = false,
                        DT_ENTERED = DateTime.UtcNow,
                        DT_MODIFIED = DateTime.UtcNow,
                        N_RESULT_VALUE = Convert.ToDecimal(chemistryList[i].ResultValue),
                        N_SAMPLE_SYSID = batchSampleID,
                        N_TEST_SYSID = testID,
                        SZ_ENTERED_BY = _modifiedBy,
                        SZ_MODIFIED_BY = _modifiedBy
                    };

                    _uow.Repository<TRN_RESULT_TB>().Add(result);
                    _uow.SaveChanges();

                    chemistryList.RemoveAt(i);
                }
            }

            Model.ChemistryDataPreviews = new List<ChemistryDataPreviewEditorViewModel>();

            foreach (var listItem in chemistryList)
            {
                Model.ChemistryDataPreviews.Add(listItem);
            }

            TempData["chemistryDataPreviewConfirm"] = Model;

            return RedirectToAction("ChemistryDataPreviewConfirm");
        }

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult ChemistryDataPreviewConfirm()
        {
            ChemistryDataPreviewViewModel model = (ChemistryDataPreviewViewModel)TempData["chemistryDataPreviewConfirm"];

            if (model.ChemistryDataPreviews.Count() > 0)
            {
                model.HasErrors = true;
                model.ErrorMessage = "These records are duplicates and did NOT save.";
            }
            else
            {
                model = new ChemistryDataPreviewViewModel()
                {
                    HasErrors = false,
                    ErrorMessage = ""
                };
            }

            return View(model);
        }

        #endregion

        #region - Ecoli Data Upload -

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult EcoliDataUpload()
        {
            EcoliDataUploadViewModel model = new EcoliDataUploadViewModel()
            {
                EcoliTestTypes = _testRepo.GetRequestGroupsForEcoli().ToList()
            };

            return View(model);
        }

        [HttpPost]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult EcoliDataUpload(EcoliDataUploadViewModel Model)
        {
            if (Model.File != null)
            {
                string name = Model.File.FileName.ToString();
                string[] parts = name.Split('.');

                if ((Model.File != null) && (Model.File.ContentLength > 0) && !string.IsNullOrEmpty(Model.File.FileName) && (parts[parts.GetUpperBound(0)] == "xlsx" || parts[parts.GetUpperBound(0)] == "xlsm"))
                {
                    byte[] uploadedFile = new byte[Model.File.InputStream.Length];
                    Model.File.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                    string fileName = Path.GetFileName(Model.File.FileName);
                    FileStream fs = System.IO.File.Create(Server.MapPath(@"~\Content\" + fileName), uploadedFile.Length, FileOptions.None);
                    BinaryWriter bw = new BinaryWriter(fs);
                    bw.Write(uploadedFile);
                    bw.Close();
                    fs.Close();
                    string path = Path.Combine(Server.MapPath(@"~/Content"), fileName).ToString();

                    List<EcoliData> ecoliDatas = EcoliExcelLinq.GetExcelEcoliData(path);

                    if (ecoliDatas != null)
                    {
                        EcoliDataPreviewViewModel model = new EcoliDataPreviewViewModel();

                        foreach (var ecoliData in ecoliDatas)
                        {
                            EcoliDataPreviewEditorViewModel preview = new EcoliDataPreviewEditorViewModel()
                            {
                                CollectedDate = ecoliData.CollectedDate,
                                CollectedTime = ecoliData.CollectedTime,
                                ResultValue = ecoliData.ReportedResult,
                                SiteID = ecoliData.SiteID,
                                TestName = ecoliData.TestName,
                                Method = ecoliData.Method,
                                Flag = ecoliData.Flag,
                                SampleMedia = ecoliData.SampleMedia,
                                SampleType = ecoliData.SampleType,
                                Units = ecoliData.Units,
                                EcoliTestID = Model.SelectedEcoliTestType.ToString(),
                                RequestGroupID = Model.SelectedEcoliTestType.ToString()
                            };
                            model.EcoliDataPreviews.Add(preview);
                        }

                        model.HasErrors = EcoliExcelLinq.ExcelDataHasErrors;
                        model.ErrorMessage = "";
                        model.ExcelFilePath = path;
                        model.SelectedTestType = Model.SelectedEcoliTestType;

                        TempData["ecoliDataPreview"] = model;

                        return RedirectToAction("EcoliDataPreview");
                    }
                    else
                    {
                        EcoliDataPreviewViewModel model = new EcoliDataPreviewViewModel();
                        model.EcoliDataPreviews = null;
                        model.HasErrors = true;
                        model.ErrorMessage = ExcelFileInfo.ExcelFileError;
                        model.ExcelFilePath = "";

                        TempData["ecoliDataPreview"] = model;

                        return RedirectToAction("EcoliDataPreview");
                    }
                }
            }

            Session["ErrorMessage"] = "The file for upload is not in the proper file format (Excel 2007 or greater with file extension .xlsx or .xlsm).";
            return RedirectToAction("InternalServerError", "Error");
        }

        #endregion

        #region - Ecoli Data Preview -

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult EcoliDataPreview()
        {
            EcoliDataPreviewViewModel model = (EcoliDataPreviewViewModel)TempData["ecoliDataPreview"];

            return View(model);
        }

        [HttpPost]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult EcoliDataPreview(EcoliDataPreviewViewModel Model)
        {
            List<EcoliDataPreviewEditorViewModel> ecoliList = Model.EcoliDataPreviews.ToList();

            for (int i = ecoliList.Count - 1; i >= 0; i--)
            //foreach (EcoliDataPreviewEditorViewModel preview in Model.EcoliDataPreviews.Reverse<EcoliDataPreviewEditorViewModel>())
            {
                string siteID = ecoliList[i].SiteID.ToString();

                int locationID = _uow.Repository<REF_LOCATION_TB>().Find(u => u.SZ_LABEL == siteID).Select(u => u.N_LOCATION_SYSID).FirstOrDefault();

                if (locationID == 0)
                {
                    REF_LOCATION_TB location = new REF_LOCATION_TB()
                    {
                        B_INACTIVE = false,
                        DT_EFFECTIVE = DateTime.UtcNow,
                        DT_ENTERED = DateTime.UtcNow,
                        DT_EXPIRED = DateTime.UtcNow,
                        DT_MODIFIED = DateTime.UtcNow,
                        SZ_DESCRIPTION = "Unknown",
                        SZ_ENTERED_BY = _modifiedBy,
                        SZ_LABEL = ecoliList[i].SiteID,
                        SZ_MODIFIED_BY = _modifiedBy
                    };

                    _uow.Repository<REF_LOCATION_TB>().Add(location);
                    _uow.SaveChanges();

                    locationID = _uow.Repository<REF_LOCATION_TB>().Find(u => u.SZ_LABEL == siteID).Select(u => u.N_LOCATION_SYSID).FirstOrDefault();
                }

                DateTime collectedDate = Convert.ToDateTime(ecoliList[i].CollectedDate);
                string collectedTime = ecoliList[i].CollectedTime.Replace(":", "");

                //int sampleID = _uow.Repository<TRN_SAMPLE_TB>().Find(u => u.N_LOCATION_SYSID == locationID && DbFunctions.TruncateTime(u.DT_COLLECTED) == DbFunctions.TruncateTime(collectedDate) && u.N_COLLECTED_TIME == collectedTime).Select(u => u.N_SAMPLE_SYSID).FirstOrDefault();
                int sampleID = 0;

                if (sampleID == 0)
                {
                    TRN_SAMPLE_TB sample = new TRN_SAMPLE_TB()
                    {
                        B_INACTIVE = false,
                        DT_COLLECTED = collectedDate,
                        DT_ENTERED = DateTime.UtcNow,
                        DT_MODIFIED = DateTime.UtcNow,
                        DT_REQUEST = DateTime.UtcNow,
                        N_COLLECTED_TIME = collectedTime,
                        N_LOCATION_SYSID = locationID,
                        SZ_BATCH_NUMBER = "1",
                        SZ_COLLECTED_BY = "Not Entered",
                        SZ_ENTERED_BY = _modifiedBy,
                        SZ_MODIFIED_BY = _modifiedBy,
                        N_REQUEST_GROUP_SYSID = Convert.ToInt32(Model.SelectedTestType)
                    };

                    _uow.Repository<TRN_SAMPLE_TB>().Add(sample);
                    _uow.SaveChanges();

                    int testID = 0;

                    if (ecoliList[i].SampleMedia == "Sediment" || ecoliList[i].SampleMedia == "Sediments") //sediment
                    {
                        testID = 11;
                    }
                    else if (ecoliList[i].SampleMedia == "Surface Water" || ecoliList[i].SampleMedia == "Water" || ecoliList[i].SampleMedia == "Others") //water
                    {
                        testID = 10;
                    }

                    sampleID = _uow.Repository<TRN_SAMPLE_TB>().Find(u => u.N_LOCATION_SYSID == locationID && DbFunctions.TruncateTime(u.DT_COLLECTED) == DbFunctions.TruncateTime(collectedDate) && u.N_COLLECTED_TIME == collectedTime).Select(u => u.N_SAMPLE_SYSID).FirstOrDefault();

                    string flag = ecoliList[i].Flag == null ? "" : ecoliList[i].Flag.ToString();

                    //int resultID = _uow.Repository<TRN_RESULT_TB>().Find(u => u.N_SAMPLE_SYSID == sampleID && u.N_TEST_SYSID == testID).Select(u => u.N_RESULT_SYSID).FirstOrDefault();
                    int resultID = 0;

                    if (resultID == 0)
                    {
                        TRN_RESULT_TB result = new TRN_RESULT_TB()
                        {
                            B_INACTIVE = false,
                            DT_ENTERED = DateTime.UtcNow,
                            DT_MODIFIED = DateTime.UtcNow,
                            N_RESULT_VALUE = Convert.ToDecimal(ecoliList[i].ResultValue),
                            N_SAMPLE_SYSID = sampleID,
                            N_TEST_SYSID = testID,
                            SZ_ENTERED_BY = _modifiedBy,
                            SZ_MODIFIED_BY = _modifiedBy,
                            SZ_RESULT_VALUE_INDICATOR = flag
                        };

                        _uow.Repository<TRN_RESULT_TB>().Add(result);
                        _uow.SaveChanges();
                    }

                    ecoliList.RemoveAt(i);
                }
            }

            Model.EcoliDataPreviews = new List<EcoliDataPreviewEditorViewModel>();

            foreach (var listItem in ecoliList)
            {
                Model.EcoliDataPreviews.Add(listItem);
            }

            TempData["ecoliDataPreviewConfirm"] = Model;

            return RedirectToAction("EcoliDataPreviewConfirm");
        }

        [HttpGet]
        [MacAuthorize(Action = "ViewPublished")]
        public ActionResult EcoliDataPreviewConfirm()
        {
            EcoliDataPreviewViewModel model = (EcoliDataPreviewViewModel)TempData["ecoliDataPreviewConfirm"];

            if (model.EcoliDataPreviews.Count() > 0)
            {
                model.HasErrors = true;
            }
            else
            {
                model = new EcoliDataPreviewViewModel()
                {
                    HasErrors = false
                };
            }

            return View(model);
        }

        #endregion

        #region - Helper Code -

        private List<SelectListItem> GetChlorinateOptions()
        {
            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem()
            {
                Value = "YES",
                Text = "YES",
                Selected = false
            });
            selectList.Add(new SelectListItem()
            {
                Value = "NO",
                Text = "NO",
                Selected = false
            });
            selectList.Add(new SelectListItem()
            {
                Value = "UNKNOWN",
                Text = "UNKNOWN",
                Selected = false
            });

            return selectList;
        }

        private List<SelectListItem> GetNPDESOptions()
        {
            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem()
            {
                Value = "YES",
                Text = "YES",
                Selected = false
            });
            selectList.Add(new SelectListItem()
            {
                Value = "NO",
                Text = "NO",
                Selected = false
            });
            selectList.Add(new SelectListItem()
            {
                Value = "UNKNOWN",
                Text = "UNKNOWN",
                Selected = false
            });

            return selectList;
        }

        private List<SelectListItem> GetActivityTypeOptions()
        {
            var selectList = new List<SelectListItem>();
            selectList.Add(new SelectListItem()
            {
                Value = "Basin",
                Text = "Basin",
                Selected = false
            });
            selectList.Add(new SelectListItem()
            {
                Value = "BOF",
                Text = "BOF",
                Selected = false
            });
            selectList.Add(new SelectListItem()
            {
                Value = "CSO-T",
                Text = "CSO-T",
                Selected = false
            });
            selectList.Add(new SelectListItem()
            {
                Value = "CSO-U",
                Text = "CSO-U",
                Selected = false
            });
            selectList.Add(new SelectListItem()
            {
                Value = "SSO",
                Text = "SSO",
                Selected = false
            });
            selectList.Add(new SelectListItem()
            {
                Value = "SSO-T",
                Text = "SSO-T",
                Selected = false
            });

            return selectList;
        }

        #endregion
    }
}