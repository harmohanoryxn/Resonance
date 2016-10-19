using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cloudmaster.WCS.WebApps.Cleaning.Models;
using WCS.Services.DataServices;
using WCS.Services.DataServices.Data;


namespace Cloudmaster.WCS.WebApps.Cleaning.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Overview()
        {
            ServerFacade _wcs = new ServerFacade();

            var locationCodes = new LocationCodes() { "WARD1" };

            var bedData = _wcs.GetCleaningBedData(locationCodes, DateTime.Now);

            var viewModel = new CleaningTableSummaryViewModel(bedData);

            return View(viewModel);
        }

        public ActionResult Details(int id)
        {
            ServerFacade _wcs = new ServerFacade();

            var locationCodes = new LocationCodes() { "WARD1" };

            var bedData = _wcs.GetCleaningBedData(locationCodes, DateTime.Now);

            var bed = bedData.FirstOrDefault(b => b.BedId == id);

            var viewModel = new CleaningTableSummaryRow(bed);

            return View(viewModel);
        }

        public ActionResult Cleaned(int id)
        {
            ServerFacade _wcs = new ServerFacade();

            var locationCodes = new LocationCodes() { "WARD1" };

            var result = _wcs.MarkBedAsClean(id, DateTime.Now, "WebApp");

            return RedirectToAction("Overview");
        }

        public ActionResult RequiresCleaning(int id)
        {
            ServerFacade _wcs = new ServerFacade();

            var locationCodes = new LocationCodes() { "WARD1" };

            var result = _wcs.MarkBedAsDirty(id, DateTime.Now, "WebApp");

            return RedirectToAction("Overview");
        }

        public ActionResult Summary()
        {
            ServerFacade _wcs = new ServerFacade();

            var locationCodes = new LocationCodes() { "WARD1" };

            var bedData = _wcs.GetCleaningBedData(locationCodes, DateTime.Now);

            var viewModel = new CleaningTableSummaryViewModel(bedData);

            return View(viewModel);
        }
    }
}
