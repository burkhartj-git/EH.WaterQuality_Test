using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EHWaterQuality.Models;
using EHWaterQuality.Utilities;

namespace EHWaterQuality.Repositories
{
    public class TestGet
    {
        /// <summary>
        /// to get all applicable tests for the given request group id
        /// </summary>
        public IEnumerable<Test> GetTestsByRequestGroupID(int RequestGroupID)
        {
            using (Entities db = new Entities())
            {
                var tests = (from rg in db.REF_REQUEST_GROUP_TB
                             join r in db.REF_REQUEST_TB
                             on rg.N_REQUEST_GROUP_SYSID
                             equals r.N_REQUEST_GROUP_SYSID
                             join tg in db.REF_TEST_GROUP_TB
                             on r.N_TEST_GROUP_SYSID
                             equals tg.N_TEST_GROUP_SYSID
                             join ttg in db.REF_TEST_X_TEST_GROUP_TB
                             on tg.N_TEST_GROUP_SYSID
                             equals ttg.N_TEST_GROUP_SYSID
                             join t in db.REF_TEST_TB
                             on ttg.N_TEST_SYSID
                             equals t.N_TEST_SYSID
                             where rg.N_REQUEST_GROUP_SYSID == RequestGroupID
                             orderby t.SZ_DESCRIPTION
                             select new Test { ID = t.N_TEST_SYSID.ToString(), Description = t.SZ_DESCRIPTION })
                            .ToList();
                return (IEnumerable<Test>)tests;
            }
        }

        /// <summary>
        /// lookup - to get test description by test id
        /// </summary>
        public string GetTestDescriptionByTestID(int TestID)
        {
            using (Entities db = new Entities())
            {
                return db.REF_TEST_TB.Where(u => u.N_TEST_SYSID == TestID).Select(u => u.SZ_DESCRIPTION).FirstOrDefault();
            }
        }

        /// <summary>
        /// to get test descriptions for dropdown
        /// </summary>
        public IEnumerable<SelectListItem> GetTestDescriptions()
        {
            //need to correct this for the chemistry request groups
            string[] tests = { "Water", "Sediment" };

            return tests.Select((u, index) => new SelectListItem { Text = u, Value = index.ToString() });
        }

        /// <summary>
        /// to get test descriptions for ecoli upload dropdown
        /// </summary>
        public IEnumerable<SelectListItem> GetRequestGroupsForEcoli()
        {
            using (Entities db = new Entities())
            {
                var requestGroups = db.REF_REQUEST_GROUP_TB.Where(u => u.SZ_DESCRIPTION.Contains("Ecoli") || u.SZ_DESCRIPTION.Contains("E. Coli")).OrderBy(u => u.SZ_DESCRIPTION).ToList();

                return requestGroups.Select(u => new SelectListItem { Value = u.N_REQUEST_GROUP_SYSID.ToString(), Text = u.SZ_DESCRIPTION });
            }
        }

        /// <summary>
        /// to get test group ids and descriptions that contain specified string attribute
        /// </summary>
        public IEnumerable<SelectListItem> GetTestGroupsBySearchString (string TestType)
        {
            //only need sediment and water for dropdown
            using (Entities db = new Entities())
            {
                IEnumerable<REF_TEST_GROUP_TB> testGroupsOrdered = (IEnumerable<REF_TEST_GROUP_TB>)db.REF_TEST_GROUP_TB
                                                    .Where(u => u.SZ_DESCRIPTION.Contains(TestType))
                                                    .OrderBy(u => u.SZ_DESCRIPTION)
                                                    .ToList();
                IEnumerable<SelectListItem> testGroups = testGroupsOrdered
                            .Select(u => new SelectListItem { Value = u.N_TEST_GROUP_SYSID.ToString(), Text = u.SZ_DESCRIPTION })
                            .Distinct()
                            .ToList();

                return testGroups;
            }
        }

        /// <summary>
        /// to get all test groups by test
        /// </summary>
        public IEnumerable<REF_TEST_X_TEST_GROUP_TB> GetTestGroupsByTestID(int TestID)
        {
            using (Entities db = new Entities())
            {
                IEnumerable<REF_TEST_X_TEST_GROUP_TB> results = (IEnumerable<REF_TEST_X_TEST_GROUP_TB>)db.REF_TEST_X_TEST_GROUP_TB
                                    .Include("REF_TEST_GROUP_TB")
                                    .Where(u => u.N_TEST_SYSID == TestID)
                                    .OrderBy(u => u.REF_TEST_GROUP_TB.SZ_DESCRIPTION)
                                    .ToList();

                return results;
            }
        }

        /// <summary>
        /// to get all tests by test group
        /// </summary>
        public IEnumerable<REF_TEST_X_TEST_GROUP_TB> GetTestsByTestGroupID(int TestGroupID)
        {
            using (Entities db = new Entities())
            {
                IEnumerable<REF_TEST_X_TEST_GROUP_TB> results = (IEnumerable<REF_TEST_X_TEST_GROUP_TB>)db.REF_TEST_X_TEST_GROUP_TB
                                    .Include("REF_TEST_TB")
                                    .Where(u => u.N_TEST_GROUP_SYSID == TestGroupID)
                                    .OrderBy(u => u.REF_TEST_TB.SZ_DESCRIPTION)
                                    .ToList();

                return results;
            }
        }

        /// <summary>
        /// to delete test x test group by id
        /// </summary>
        public bool DeleteTestByTestGroupByIDs(int TestID, int TestGroupID)
        {
            using (Entities db = new Entities())
            {
                bool isDeleted = false;

                REF_TEST_X_TEST_GROUP_TB result = db.REF_TEST_X_TEST_GROUP_TB
                                        .FirstOrDefault(u => u.N_TEST_SYSID == TestID && u.N_TEST_GROUP_SYSID == TestGroupID);

                db.REF_TEST_X_TEST_GROUP_TB.Remove(result);
                db.SaveChanges();
                isDeleted = true;

                return isDeleted;
            }
        }

        /// <summary>
        /// to get select list for new test groups for test
        /// </summary>
        public IEnumerable<SelectListItem> GetTestGroupsSelectListByTestID(int TestID)
        {
            using (Entities db = new Entities())
            {
                List<int> testGroupIDs = (List<int>)db.REF_TEST_X_TEST_GROUP_TB
                                    .Where(u => u.N_TEST_SYSID == TestID)
                                    .Select(u => u.N_TEST_GROUP_SYSID)
                                    .ToList();

                IEnumerable<REF_TEST_GROUP_TB> resultsOrdered = (IEnumerable<REF_TEST_GROUP_TB>)(from r in db.REF_TEST_GROUP_TB
                                                                                                         where !testGroupIDs.Contains(r.N_TEST_GROUP_SYSID)
                                                                                                         orderby r.SZ_DESCRIPTION
                                                                                                         select r).ToList();

                IEnumerable<SelectListItem> results = resultsOrdered
                                    .Select(u => new SelectListItem { Value = u.N_TEST_GROUP_SYSID.ToString(), Text = u.SZ_DESCRIPTION })
                                    .Distinct()
                                    .ToList();

                return results;
            }
        }

        /// <summary>
        /// to get select list for new tests for test groups
        /// </summary>
        public IEnumerable<SelectListItem> GetTestsSelectListByTestGroupID(int TestGroupID)
        {
            using (Entities db = new Entities())
            {
                List<int> testIDs = (List<int>)db.REF_TEST_X_TEST_GROUP_TB
                                    .Where(u => u.N_TEST_GROUP_SYSID == TestGroupID)
                                    .Select(u => u.N_TEST_SYSID)
                                    .ToList();

                IEnumerable<REF_TEST_TB> resultsOrdered = (IEnumerable<REF_TEST_TB>)(from r in db.REF_TEST_TB
                                                                                                 where !testIDs.Contains(r.N_TEST_SYSID)
                                                                                                 orderby r.SZ_DESCRIPTION
                                                                                                 select r).ToList();

                IEnumerable<SelectListItem> results = resultsOrdered
                                    .Select(u => new SelectListItem { Value = u.N_TEST_SYSID.ToString(), Text = u.SZ_DESCRIPTION })
                                    .Distinct()
                                    .ToList();

                return results;
            }
        }

        /// <summary>
        /// to get select list for all test groups
        /// </summary>
        public IEnumerable<SelectListItem> GetTestGroupsSelectList()
        {
            using (Entities db = new Entities())
            {
                IEnumerable<SelectListItem> resultsOrdered = (IEnumerable<SelectListItem>)db.REF_TEST_GROUP_TB
                                    .OrderBy(u => u.SZ_DESCRIPTION)
                                    .ToList()
                                    .Select(u => new SelectListItem { Value = u.N_TEST_GROUP_SYSID.ToString(), Text = u.SZ_DESCRIPTION })
                                    .Distinct()
                                    .ToList();

                return resultsOrdered;
            }
        }

        /// <summary>
        /// to check if the test group description exists
        /// </summary>
        public bool IsTestGroupDescriptionDuplicate(string TestGroupDescription)
        {
            using (Entities db = new Entities())
            {
                return db.REF_TEST_GROUP_TB.Any(r => r.SZ_DESCRIPTION == TestGroupDescription);
            }
        }

        /// <summary>
        /// to get test group description by id
        /// </summary>
        public string GetTestGroupDescriptionByID(int ID)
        {
            using (Entities db = new Entities())
            {
                if (ID == 0)
                {
                    return "";
                }
                else
                {
                    return db.REF_TEST_GROUP_TB.Where(u => u.N_TEST_GROUP_SYSID == ID).Select(u => u.SZ_DESCRIPTION).FirstOrDefault().ToString();
                }
            }
        }
    }
}