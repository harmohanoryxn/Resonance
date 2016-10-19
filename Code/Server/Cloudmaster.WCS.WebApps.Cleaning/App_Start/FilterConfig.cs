using System.Web;
using System.Web.Mvc;

namespace Cloudmaster.WCS.WebApps.Cleaning
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}