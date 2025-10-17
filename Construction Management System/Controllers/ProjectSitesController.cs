// Created By: Srikhar Dogiparthy
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Dogiparthy_Spring25.Data;
using Dogiparthy_Spring25.Models;
using Microsoft.AspNetCore.Authorization;
using Dogiparthy_Spring25.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Identity;

namespace Dogiparthy_Spring25.Controllers
{
    [Authorize]

    public class ProjectSitesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ProjectSitesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize(Roles = "Admin,StandardUser")]

        public async Task<IActionResult> Index()
        {
            var sessionClicks = HttpContext.Session.Get<List<ProjectSite>>("UserSiteClicks");
            if (sessionClicks != null)
            {
                ViewBag.UserSiteClicks = sessionClicks;
            }

            IQueryable<ProjectSite> query = _context.ProjectSites
                .Include(p => p.CurrentOwner)
                .Include(p => p.WorkOrders)
                    .ThenInclude(wo => wo.WorkAllocations);

            // Role-based filtering
            if (User.IsInRole("StandardUser"))
            {
                var userId = _userManager.GetUserId(User);
                var currentUser = _context.People.FirstOrDefault(p => p.IdentityUserId == userId);

                if (currentUser != null)
                {
                    // Check if user is a CurrentOwner
                    var owner = _context.CurrentOwners.FirstOrDefault(co => co.PersonID == currentUser.PersonID);
                    if (owner != null)
                    {
                        // CurrentOwner can only see their own properties
                        query = query.Where(ps => ps.CurrentOwnerPersonID == owner.PersonID);
                    }
                    else
                    {
                        // SiteWorker can see properties with their assigned work orders
                        var worker = _context.SiteWorkers.FirstOrDefault(sw => sw.PersonID == currentUser.PersonID);
                        if (worker != null)
                        {
                            query = query.Where(ps => ps.WorkOrders.Any(wo => wo.WorkAllocations.Any(wa => wa.SiteWorkerPersonID == worker.PersonID)));
                        }
                    }
                }
            }
            // Admin can see all

            return View(await query.ToListAsync());
        }
        [Authorize(Roles = "Admin,StandardUser")]

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var projectSite = await _context.ProjectSites
                .Include(o => o.CurrentOwner)
                .Include(ps => ps.WorkOrders)
                    .ThenInclude(wo => wo.WorkAllocations)
                .FirstOrDefaultAsync(m => m.SiteID == id);

            if (projectSite == null) return NotFound();

            // Role-based access control
            if (User.IsInRole("StandardUser"))
            {
                var userId = _userManager.GetUserId(User);
                var currentUser = _context.People.FirstOrDefault(p => p.IdentityUserId == userId);

                if (currentUser != null)
                {
                    // Check if user is a CurrentOwner
                    var owner = _context.CurrentOwners.FirstOrDefault(co => co.PersonID == currentUser.PersonID);
                    if (owner != null)
                    {
                        // CurrentOwner can only view their own properties
                        if (projectSite.CurrentOwnerPersonID != owner.PersonID)
                            return Forbid();
                    }
                    else
                    {
                        // SiteWorker can only view properties with their assigned work orders
                        var worker = _context.SiteWorkers.FirstOrDefault(sw => sw.PersonID == currentUser.PersonID);
                        if (worker != null)
                        {
                            var hasAccess = projectSite.WorkOrders != null && projectSite.WorkOrders.Any(wo => wo.WorkAllocations != null && wo.WorkAllocations.Any(wa => wa.SiteWorkerPersonID == worker.PersonID));
                            if (!hasAccess)
                                return Forbid();
                        }
                    }
                }
            }

            AddClickedSiteToSession(projectSite);
            return View(projectSite);
        }

        [Authorize(Roles ="Admin")]
        public IActionResult Create()
        {
            ViewBag.SiteTypes = new SelectList(Enum.GetValues(typeof(SiteType)));
            ViewData["CurrentOwnerPersonID"] = new SelectList(_context.CurrentOwners, "PersonID", "LastName");
            return View();
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SiteID,SiteTitle,Location,SizeSqFt,SiteType,CurrentOwnerPersonID")] ProjectSite projectSite)
        {
            if (ModelState.IsValid)
            {
                _context.Add(projectSite);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CurrentOwnerPersonID"] = new SelectList(_context.CurrentOwners, "PersonID", "LastName");
            ViewBag.SiteTypes = new SelectList(Enum.GetValues(typeof(SiteType)));
            AddClickedSiteToSession(projectSite);
            return View(projectSite);
        }

        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var projectSite = await _context.ProjectSites.FindAsync(id);
            if (projectSite == null) return NotFound();

            ViewData["CurrentOwnerPersonID"] = new SelectList(_context.CurrentOwners, "PersonID", "LastName");
            ViewBag.SiteTypes = new SelectList(Enum.GetValues(typeof(SiteType)));
            AddClickedSiteToSession(projectSite);
            return View(projectSite);
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SiteID,SiteTitle,Location,SizeSqFt,SiteType,CurrentOwnerPersonID")] ProjectSite projectSite)
        {
            if (id != projectSite.SiteID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projectSite);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectSiteExists(projectSite.SiteID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CurrentOwnerPersonID"] = new SelectList(_context.CurrentOwners, "PersonID", "LastName");
            ViewBag.SiteTypes = new SelectList(Enum.GetValues(typeof(SiteType)));
            AddClickedSiteToSession(projectSite);
            return View(projectSite);
        }

        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var projectSite = await _context.ProjectSites.FirstOrDefaultAsync(m => m.SiteID == id);
            if (projectSite == null) return NotFound();

            return View(projectSite);
        }

        [Authorize(Roles ="Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projectSite = await _context.ProjectSites.FirstOrDefaultAsync(m => m.SiteID == id);


            //if (projectSite != null)
            //{
            //    ViewData["ErrorMessage"] = "You cannot delete Project Site with Site Work Order(s) assigned.";

            //    return View("Delete", projectSite);
            //}

             _context.ProjectSites.Remove(projectSite);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectSiteExists(int id)
        {
            return _context.ProjectSites.Any(e => e.SiteID == id);
        }
      
        private void AddClickedSiteToSession(ProjectSite site)
        {
            var sessionClicks = HttpContext.Session.Get<List<ProjectSite>>("UserSiteClicks") ?? new List<ProjectSite>();
            if (!sessionClicks.Any(o => o.SiteID == site.SiteID))
            {
                sessionClicks.Add(site);
                HttpContext.Session.Set("UserSiteClicks", sessionClicks);
            }
        }

        [Authorize(Roles ="Admin,StandardUser")]
        [HttpGet]
        public IActionResult Search()
        {
            var siteSearch = new ProjectSiteSearchViewModel();
            return View(siteSearch);
        }

        [Authorize(Roles ="Admin,StandardUser")]
        [HttpPost]
        public IActionResult Search(ProjectSiteSearchViewModel siteSearch)
        {
            if (!string.IsNullOrWhiteSpace(siteSearch.ProjectSite.SiteTitle))
            {
                var resultList = _context.ProjectSites
                                  .Where(ps => ps.SiteTitle.ToLower()
                                  .Contains(siteSearch.ProjectSite.SiteTitle.ToLower()))
                                  .ToList();

                if (resultList.Count == 1)
                {
                    return RedirectToAction("Details", "ProjectSites",
                        new { id = resultList[0].SiteID });
                }
                else if (resultList.Count < 1)
                {
                    siteSearch.SearchError = "No Project Sites found with the search criteria provided.";
                }

                siteSearch.ResultList = resultList;
            }
            else
            {
                siteSearch.SearchError = "Please enter a Site Title to search.";
            }

            return View(siteSearch);
        }

    }
}