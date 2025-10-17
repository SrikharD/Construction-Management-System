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
using Dogiparthy_Spring25.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Dogiparthy_Spring25.Controllers
{
    [Authorize]

    public class WorkOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public WorkOrdersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin,StandardUser")]
        // GET: WorkOrders
        public async Task<IActionResult> Index()
        {
            var sessionClicks = HttpContext.Session.Get<List<WorkOrder>>("UserWorkClicks");

            if (sessionClicks != null)
            {
                ViewBag.UserWorkClicks = sessionClicks;
            }

            IQueryable<WorkOrder> query = _context.WorkOrders.Include(w => w.ProjectSite);

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
                        // CurrentOwner can only see work orders for their properties
                        query = query.Where(wo => wo.ProjectSite.CurrentOwnerPersonID == owner.PersonID);
                    }
                    else
                    {
                        // SiteWorker can only see work orders assigned to them
                        var worker = _context.SiteWorkers.FirstOrDefault(sw => sw.PersonID == currentUser.PersonID);
                        if (worker != null)
                        {
                            query = query.Where(wo => wo.WorkAllocations.Any(wa => wa.SiteWorkerPersonID == worker.PersonID));
                        }
                    }
                }
            }
            // Admin can see all

            var applicationDbContext = query;
            return View(await applicationDbContext.ToListAsync());
        }
        [Authorize(Roles = "Admin,StandardUser")]

        // GET: WorkOrders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workOrder = await _context.WorkOrders
                .Include(w => w.ProjectSite)
                .FirstOrDefaultAsync(m => m.WorkOrderID == id);
            if (workOrder == null)
            {
                return NotFound();
            }
            ViewBag.TaskStatus = new SelectList(Enum.GetValues(typeof(Dogiparthy_Spring25.Models.TaskStatus)));
            AddClickedworkToSession(workOrder);
            return View(workOrder);
        }

        [Authorize(Roles ="Admin")]
        // GET: WorkOrders/Create
        public IActionResult Create()
        {
            ViewData["PropertyID"] = new SelectList(_context.ProjectSites, "SiteID", "SiteTitle");
            ViewBag.TaskStatus = new SelectList(Enum.GetValues(typeof(Dogiparthy_Spring25.Models.TaskStatus)));
            return View();
        }

        [Authorize(Roles ="Admin")]
        // POST: WorkOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WorkOrderID,TaskName,Description,Status,EstimatedEndDate,PropertyID")] WorkOrder workOrder)
        {
            if (ModelState.IsValid)
            {
                // Map PropertyID to ProjectSiteSiteID for proper foreign key relationship
                workOrder.ProjectSiteSiteID = workOrder.PropertyID;
                _context.Add(workOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PropertyID"] = new SelectList(_context.ProjectSites, "SiteID", "SiteTitle", workOrder.PropertyID);
            ViewBag.TaskStatus = new SelectList(Enum.GetValues(typeof(Dogiparthy_Spring25.Models.TaskStatus)));
            return View(workOrder);
        }

        [Authorize(Roles ="Admin")]
        // GET: WorkOrders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workOrder = await _context.WorkOrders.FindAsync(id);
            if (workOrder == null)
            {
                return NotFound();
            }
            ViewData["PropertyID"] = new SelectList(_context.ProjectSites, "SiteID", "SiteTitle", workOrder.PropertyID);
            ViewBag.TaskStatus = new SelectList(Enum.GetValues(typeof(Dogiparthy_Spring25.Models.TaskStatus)));
            AddClickedworkToSession(workOrder);

            return View(workOrder);
        }

        [Authorize(Roles ="Admin")]
        // POST: WorkOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WorkOrderID,TaskName,Description,Status,EstimatedEndDate,PropertyID")] WorkOrder workOrder)
        {
            if (id != workOrder.WorkOrderID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the original work order to check if status is changing to Completed
                    var originalWorkOrder = await _context.WorkOrders
                        .Include(wo => wo.WorkAllocations)
                        .ThenInclude(wa => wa.SiteWorker)
                        .FirstOrDefaultAsync(wo => wo.WorkOrderID == id);

                    if (originalWorkOrder == null)
                    {
                        return NotFound();
                    }

                    // Check if status is being changed to Completed
                    if (originalWorkOrder.Status != Models.TaskStatus.Completed &&
                        workOrder.Status == Models.TaskStatus.Completed)
                    {
                        // Get all SiteWorkers assigned to this work order
                        var assignedWorkers = originalWorkOrder.WorkAllocations
                            .Where(wa => wa.SiteWorker != null)
                            .Select(wa => wa.SiteWorker)
                            .ToList();

                        // Set all assigned workers to Available
                        foreach (var worker in assignedWorkers)
                        {
                            worker.IsAvailable = true;
                        }
                    }

                    // Update the tracked entity with new values
                    originalWorkOrder.TaskName = workOrder.TaskName;
                    originalWorkOrder.Description = workOrder.Description;
                    originalWorkOrder.Status = workOrder.Status;
                    originalWorkOrder.EstimatedEndDate = workOrder.EstimatedEndDate;
                    originalWorkOrder.PropertyID = workOrder.PropertyID;
                    originalWorkOrder.ProjectSiteSiteID = workOrder.PropertyID;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WorkOrderExists(workOrder.WorkOrderID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["PropertyID"] = new SelectList(_context.ProjectSites, "SiteID", "SiteTitle", workOrder.PropertyID);
            ViewBag.TaskStatus = new SelectList(Enum.GetValues(typeof(Dogiparthy_Spring25.Models.TaskStatus)));
            AddClickedworkToSession(workOrder);

            return View(workOrder);
        }

        [Authorize(Roles ="Admin")]
        // GET: WorkOrders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var workOrder = await _context.WorkOrders
                .Include(w => w.ProjectSite)
                .FirstOrDefaultAsync(m => m.WorkOrderID == id);
            if (workOrder == null)
            {
                return NotFound();
            }

            return View(workOrder);
        }

        [Authorize(Roles ="Admin")]
        // POST: WorkOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var workOrder = await _context.WorkOrders
                               .Include(w => w.WorkAllocations)
                               .FirstOrDefaultAsync(w => w.WorkOrderID == id);


            if (workOrder.WorkAllocations.Any())
            {
                ViewData["ErrorMessage"] = "You cannot delete Site Work Order with Site Worker(s) assigned.";

                return View("Delete", workOrder);
                
            }

            _context.WorkOrders.Remove(workOrder);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void AddClickedworkToSession(WorkOrder work)
        {
            var sessionClicks = HttpContext.Session.Get<List<WorkOrder>>("UserWorkClicks");

            if (sessionClicks == null)
            {
                sessionClicks = new List<WorkOrder>();
            }

            var workInSession = sessionClicks.FirstOrDefault(o => o.WorkOrderID == work.WorkOrderID);

            if (workInSession == null)
            {

                sessionClicks.Add(work);
                HttpContext.Session.Set<List<WorkOrder>>("UserWorkClicks", sessionClicks);
            }
        }

        // GET: Assign SiteWorkers to a WorkOrder
        public IActionResult AssignSiteWorkers(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            // Check if work order is completed
            var workOrder = _context.WorkOrders.FirstOrDefault(wo => wo.WorkOrderID == id);
            if (workOrder != null && workOrder.Status == Models.TaskStatus.Completed)
            {
                TempData["ErrorMessage"] = "Cannot assign workers to a completed work order.";
                return RedirectToAction("Details", new { id = id });
            }

            var assignedWorkers = _context.WorkAllocation
                                    .Include(wa => wa.SiteWorker)
                                    .Where(wa => wa.WorkOrderID == id)
                                    .ToList();

            var allWorkers = _context.SiteWorkers
                               .Where(w => w.IsAvailable)
                               .ToList();

            List<SiteWorker> availableWorkers = new List<SiteWorker>();

            foreach (var worker in allWorkers)
            {
                bool isAvailable = true;

                foreach (var alloc in assignedWorkers)
                {
                    if (alloc.SiteWorkerPersonID == worker.PersonID)
                    {
                        isAvailable = false;
                    }
                }

                if (isAvailable)
                {
                    availableWorkers.Add(worker);
                }
            }

            List<WorkOrder_AddSiteWorker_ViewModel> model = new List<WorkOrder_AddSiteWorker_ViewModel>();

            foreach (var worker in availableWorkers)
            {
                WorkOrder_AddSiteWorker_ViewModel vm = new WorkOrder_AddSiteWorker_ViewModel();
                vm.SiteWorker = worker;
                vm.Selected = false;
                model.Add(vm);
            }

            ViewBag.WorkOrderID = id;

            return View(model);
        }

        [HttpPost]
        public IActionResult AssignSiteWorkers(int? id, List<WorkOrder_AddSiteWorker_ViewModel> AssignSiteWorkers)
        {
            foreach (var item in AssignSiteWorkers)
            {
                if (item.Selected)
                {
                    WorkAllocation workAlloc = new WorkAllocation();
                    workAlloc.WorkOrderID = id;
                    workAlloc.SiteWorkerPersonID = item.SiteWorker.PersonID;
                    workAlloc.AssignedDate = DateTime.Now;

                    _context.WorkAllocation.Add(workAlloc);

                    // Update SiteWorker's IsAvailable status to false
                    var siteWorker = _context.SiteWorkers.FirstOrDefault(sw => sw.PersonID == item.SiteWorker.PersonID);
                    if (siteWorker != null)
                    {
                        siteWorker.IsAvailable = false;
                        _context.Update(siteWorker);
                    }
                }
            }

            _context.SaveChanges();
            return RedirectToAction("ManageSiteWorkers", new { id = id });
        }

        // GET: View all assigned SiteWorkers for a WorkOrder
        public IActionResult ManageSiteWorkers(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Id = id;

            // Check if work order is completed
            var workOrder = _context.WorkOrders.FirstOrDefault(wo => wo.WorkOrderID == id);
            if (workOrder != null && workOrder.Status == Models.TaskStatus.Completed)
            {
                TempData["ErrorMessage"] = "Cannot manage workers for a completed work order.";
                return RedirectToAction("Details", new { id = id });
            }

            var assignedSiteWorkers = _context.WorkAllocation
                                        .Include(wa => wa.SiteWorker)
                                        .Include(wa => wa.WorkOrder)
                                        .Where(wa => wa.WorkOrderID == id)
                                        .ToList();

            if (assignedSiteWorkers.Count < 1)
            {
                return RedirectToAction("AssignSiteWorkers", new { id = id });
            }

            return View(assignedSiteWorkers);
        }

        public IActionResult DeleteSiteWorker(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var workAllocation = _context.WorkAllocation
                                  .Include(wa => wa.SiteWorker)
                                  .Include(wa => wa.WorkOrder)
                                  .SingleOrDefault(wa => wa.WorkID == id);

            if (workAllocation == null)
            {
                return NotFound();
            }

            return View(workAllocation);
        }

        [HttpPost, ActionName("DeleteSiteWorker")]
        public IActionResult DeleteSiteWorkerConfirmed(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var workAlloc = _context.WorkAllocation
                              .Include(wa => wa.SiteWorker)
                              .SingleOrDefault(wa => wa.WorkID == id);

            int? siteWorkerPersonID = workAlloc?.SiteWorkerPersonID;
            int? workOrderID = workAlloc?.WorkOrderID;

            _context.WorkAllocation.Remove(workAlloc);
            _context.SaveChanges();

            // Check if the SiteWorker has any remaining work allocations
            if (siteWorkerPersonID.HasValue)
            {
                var remainingAllocations = _context.WorkAllocation
                                            .Where(wa => wa.SiteWorkerPersonID == siteWorkerPersonID)
                                            .Any();

                // If no remaining allocations, set IsAvailable back to true
                if (!remainingAllocations)
                {
                    var siteWorker = _context.SiteWorkers.FirstOrDefault(sw => sw.PersonID == siteWorkerPersonID);
                    if (siteWorker != null)
                    {
                        siteWorker.IsAvailable = true;
                        _context.Update(siteWorker);
                        _context.SaveChanges();
                    }
                }
            }

            return RedirectToAction("ManageSiteWorkers", new { id = workOrderID });
        }



        private bool WorkOrderExists(int id)
        {
            return _context.WorkOrders.Any(e => e.WorkOrderID == id);
        }
    }
}
