// Created By: Srikhar Dogiparthy
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Dogiparthy_Spring25.Models;
using System.Collections.Generic;

namespace Dogiparthy_Spring25.Controllers
{
    [Authorize]
    public class HistoryController : Controller
    {
        // GET: History/Index
        public IActionResult Index()
        {
            // Get all session history data
            var workOrderHistory = HttpContext.Session.Get<List<WorkOrder>>("UserWorkClicks") ?? new List<WorkOrder>();
            var projectSiteHistory = HttpContext.Session.Get<List<ProjectSite>>("UserSiteClicks") ?? new List<ProjectSite>();
            var siteWorkerHistory = HttpContext.Session.Get<List<SiteWorker>>("UserWorkerClicks") ?? new List<SiteWorker>();
            var currentOwnerHistory = HttpContext.Session.Get<List<CurrentOwner>>("UserOwnerClicks") ?? new List<CurrentOwner>();

            // Create a view model to hold all history data
            var historyData = new Dictionary<string, object>
            {
                { "WorkOrders", workOrderHistory ?? new List<WorkOrder>() },
                { "ProjectSites", projectSiteHistory ?? new List<ProjectSite>() },
                { "SiteWorkers", siteWorkerHistory ?? new List<SiteWorker>() },
                { "CurrentOwners", currentOwnerHistory ?? new List<CurrentOwner>() }
            };

            return View(historyData);
        }

        // GET: History/ClearHistory
        public IActionResult ClearHistory()
        {
            return View();
        }

        // POST: History/ClearHistory
        [HttpPost, ActionName("ClearHistory")]
        public IActionResult ClearHistoryConfirmed()
        {
            // Clear all session history
            HttpContext.Session.Remove("UserWorkClicks");
            HttpContext.Session.Remove("UserSiteClicks");
            HttpContext.Session.Remove("UserWorkerClicks");
            HttpContext.Session.Remove("UserOwnerClicks");

            TempData["SuccessMessage"] = "All history has been cleared successfully.";
            return RedirectToAction("Index");
        }

        // GET: History/ClearWorkOrderHistory
        public IActionResult ClearWorkOrderHistory()
        {
            HttpContext.Session.Remove("UserWorkClicks");
            TempData["SuccessMessage"] = "Work Order history has been cleared.";
            return RedirectToAction("Index");
        }

        // GET: History/ClearProjectSiteHistory
        public IActionResult ClearProjectSiteHistory()
        {
            HttpContext.Session.Remove("UserSiteClicks");
            TempData["SuccessMessage"] = "Project Site history has been cleared.";
            return RedirectToAction("Index");
        }

        // GET: History/ClearSiteWorkerHistory
        public IActionResult ClearSiteWorkerHistory()
        {
            HttpContext.Session.Remove("UserWorkerClicks");
            TempData["SuccessMessage"] = "Site Worker history has been cleared.";
            return RedirectToAction("Index");
        }

        // GET: History/ClearCurrentOwnerHistory
        public IActionResult ClearCurrentOwnerHistory()
        {
            HttpContext.Session.Remove("UserOwnerClicks");
            TempData["SuccessMessage"] = "Current Owner history has been cleared.";
            return RedirectToAction("Index");
        }
    }
}

