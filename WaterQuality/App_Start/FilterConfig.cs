using EHWaterQuality.Filters;
using System.Web;
using System.Web.Mvc;

namespace EHWaterQuality.Models
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new Log4NetException());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
