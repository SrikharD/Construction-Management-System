// Created By: Srikhar Dogiparthy
using System.Diagnostics;
using Dogiparthy_Spring25.Data;
using Dogiparthy_Spring25.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dogiparthy_Spring25.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            // Only show dashboards if user is authenticated
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);

                // Check if user is a SiteWorker
                var siteWorker = _context.SiteWorkers.FirstOrDefault(sw => sw.IdentityUserId == userId);
                if (siteWorker != null)
                {
                    // Get all work orders assigned to this worker
                    var assignedWorkOrders = _context.WorkOrders
                        .Include(wo => wo.ProjectSite)
                            .ThenInclude(ps => ps.CurrentOwner)
                        .Where(wo => wo.WorkAllocations.Any(wa => wa.SiteWorkerPersonID == siteWorker.PersonID))
                        .ToList();

                    // Get unique project sites from assigned work orders
                    var assignedProjectSites = assignedWorkOrders
                        .Select(wo => wo.ProjectSite)
                        .Distinct()
                        .ToList();

                    // Create a view model for the dashboard
                    var dashboardData = new
                    {
                        WorkerName = siteWorker.FirstName + " " + siteWorker.LastName,
                        AssignedWorkOrders = assignedWorkOrders,
                        AssignedProjectSites = assignedProjectSites,
                        TotalWorkOrders = assignedWorkOrders.Count,
                        TotalProjectSites = assignedProjectSites.Count,
                        CompletedWorkOrders = assignedWorkOrders.Count(wo => wo.Status == Models.TaskStatus.Completed),
                        InProgressWorkOrders = assignedWorkOrders.Count(wo => wo.Status == Models.TaskStatus.InProgress)
                    };

                    ViewBag.DashboardData = dashboardData;
                    ViewBag.IsSiteWorker = true;
                    ViewBag.TaskStatus = typeof(Models.TaskStatus);
                    return View();
                }

                // Check if user is a CurrentOwner
                var currentOwner = _context.CurrentOwners
                    .FirstOrDefault(co => co.IdentityUserId == userId);

                if (currentOwner != null)
                {
                    // Get all project sites owned by this owner
                    var ownedProjectSites = _context.ProjectSites
                        .Include(ps => ps.WorkOrders)
                            .ThenInclude(wo => wo.WorkAllocations)
                        .Where(ps => ps.CurrentOwnerPersonID == currentOwner.PersonID)
                        .ToList();

                    // Get all work orders from owned project sites
                    var allWorkOrders = ownedProjectSites
                        .SelectMany(ps => ps.WorkOrders)
                        .ToList();

                    // Create a view model for the CurrentOwner dashboard
                    var ownerDashboardData = new
                    {
                        OwnerName = currentOwner.FirstName + " " + currentOwner.LastName,
                        OwnedProjectSites = ownedProjectSites,
                        AllWorkOrders = allWorkOrders,
                        TotalProjectSites = ownedProjectSites.Count,
                        TotalWorkOrders = allWorkOrders.Count,
                        CompletedWorkOrders = allWorkOrders.Count(wo => wo.Status == Models.TaskStatus.Completed),
                        InProgressWorkOrders = allWorkOrders.Count(wo => wo.Status == Models.TaskStatus.InProgress),
                        NotStartedWorkOrders = allWorkOrders.Count(wo => wo.Status == Models.TaskStatus.NotStarted),
                        OnHoldWorkOrders = allWorkOrders.Count(wo => wo.Status == Models.TaskStatus.OnHold)
                    };

                    ViewBag.OwnerDashboardData = ownerDashboardData;
                    ViewBag.IsCurrentOwner = true;
                    ViewBag.TaskStatus = typeof(Models.TaskStatus);
                    return View();
                }
            }

            return View();
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Help()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Helper method to get the current user's Person record
        /// </summary>
        protected Person GetCurrentUserPerson()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return null;

            return _context.People.FirstOrDefault(p => p.IdentityUserId == userId);
        }

        /// <summary>
        /// Helper method to get the current user as CurrentOwner
        /// </summary>
        protected CurrentOwner GetCurrentUserAsOwner()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return null;

            return _context.CurrentOwners.FirstOrDefault(co => co.IdentityUserId == userId);
        }

        /// <summary>
        /// Helper method to get the current user as SiteWorker
        /// </summary>
        protected SiteWorker GetCurrentUserAsWorker()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
                return null;

            return _context.SiteWorkers.FirstOrDefault(sw => sw.IdentityUserId == userId);
        }
    }
}
