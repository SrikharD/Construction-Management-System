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
using Microsoft.Build.Evaluation;
using Microsoft.AspNetCore.Identity;

namespace Dogiparthy_Spring25.Controllers
{
    [Authorize]
    public class SiteWorkersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SiteWorkersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        private void AddClickedWorkerToSession(SiteWorker worker)
        {
            var sessionClicks = HttpContext.Session.Get<List<SiteWorker>>("UserWorkerClicks");

            if (sessionClicks == null)
            {
                sessionClicks = new List<SiteWorker>();
            }

            var workerInSession = sessionClicks.FirstOrDefault(o => o.PersonID == worker.PersonID);

            if (workerInSession == null)
            {

                sessionClicks.Add(worker);
                HttpContext.Session.Set<List<SiteWorker>>("UserWorkerClicks", sessionClicks);
            }
        }
        [Authorize(Roles = "Admin,StandardUser")]
        // GET: SiteWorkers
        public async Task<IActionResult> Index()
        {
            var sessionClicks = HttpContext.Session.Get<List<SiteWorker>>("UserWorkerClicks");

            if (sessionClicks != null)
            {
                ViewBag.UserWorkerClicks = sessionClicks;
            }

            IQueryable<SiteWorker> query = _context.SiteWorkers;

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
                        // CurrentOwner can see workers assigned to their properties
                        query = query.Where(sw => sw.WorkAllocations.Any(wa => wa.WorkOrder.ProjectSite.CurrentOwnerPersonID == owner.PersonID));
                    }
                    else
                    {
                        // SiteWorker can only see themselves
                        var worker = _context.SiteWorkers.FirstOrDefault(sw => sw.PersonID == currentUser.PersonID);
                        if (worker != null)
                        {
                            query = query.Where(sw => sw.PersonID == worker.PersonID);
                        }
                    }
                }
            }
            // Admin can see all

            return View(await query.ToListAsync());
        }

        [Authorize(Roles = "Admin,StandardUser")]
        // GET: SiteWorkers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var siteWorker = await _context.SiteWorkers
                .FirstOrDefaultAsync(m => m.PersonID == id);
            if (siteWorker == null)
            {
                return NotFound();
            }

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
                        // CurrentOwner can only view workers assigned to their properties
                        var hasAccess = siteWorker.WorkAllocations.Any(wa => wa.WorkOrder.ProjectSite.CurrentOwnerPersonID == owner.PersonID);
                        if (!hasAccess)
                            return Forbid();
                    }
                    else
                    {
                        // SiteWorker can only view themselves
                        if (siteWorker.PersonID != currentUser.PersonID)
                            return Forbid();
                    }
                }
            }

            AddClickedWorkerToSession(siteWorker);
            return View(siteWorker);
        }

        [Authorize(Roles ="Admin")]
        // GET: SiteWorkers/Create
        public IActionResult Create()
        {
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "UserName");
            return View();
        }

        [Authorize(Roles ="Admin")]
        // POST: SiteWorkers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StartDate,HourlyWage,IsAvailable,PersonID,FirstName,LastName,Email,PhoneNumber,Address1,Address2,City,State,ZipCode,DOB,IdentityUserId")] SiteWorker siteWorker)
        {
            if (siteWorker.DOB > DateTime.Today || siteWorker.DOB > siteWorker.StartDate)
            {
                TempData["DOBError"] = "Date of Birth cannot be in the future (or) cannot hire in past.";
                return View(siteWorker);
                //return RedirectToAction(nameof(Create));
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Automatically create a StandardUser account for the Site Worker
                    var newUser = new IdentityUser
                    {
                        UserName = siteWorker.Email,
                        Email = siteWorker.Email,
                        EmailConfirmed = true
                    };

                    // Use standard password for all new users
                    string tempPassword = "Password1!";

                    // Create the user
                    var result = await _userManager.CreateAsync(newUser, tempPassword);

                    if (result.Succeeded)
                    {
                        // Assign StandardUser role
                        await _userManager.AddToRoleAsync(newUser, "StandardUser");

                        // Link the user to the SiteWorker
                        siteWorker.IdentityUserId = newUser.Id;

                        // Add the SiteWorker to the database
                        _context.Add(siteWorker);
                        await _context.SaveChangesAsync();

                        AddClickedWorkerToSession(siteWorker);

                        TempData["SuccessMessage"] = $"Site Worker created successfully! A StandardUser account has been created with username: {newUser.UserName}. Password: Password1!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        // If user creation failed, show errors
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                }
            }
            AddClickedWorkerToSession(siteWorker);
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "UserName", siteWorker.IdentityUserId);
            return View(siteWorker);
        }

        [Authorize(Roles ="Admin")]
        // GET: SiteWorkers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var siteWorker = await _context.SiteWorkers.FindAsync(id);
            if (siteWorker == null)
            {
                return NotFound();
            }
            AddClickedWorkerToSession(siteWorker);
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "UserName", siteWorker.IdentityUserId);
            return View(siteWorker);
        }

        [Authorize(Roles ="Admin")]
        // POST: SiteWorkers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StartDate,HourlyWage,IsAvailable,PersonID,FirstName,LastName,Email,PhoneNumber,Address1,Address2,City,State,ZipCode,DOB,IdentityUserId")] SiteWorker siteWorker)
        {
            if (id != siteWorker.PersonID)
            {
                return NotFound();
            }
            if (siteWorker.DOB > DateTime.Today && siteWorker.DOB > siteWorker.StartDate)
            {
                TempData["DOBError"] = "Date of Birth cannot be in the future (or) cannot hire in past.";
                return View(siteWorker);
                //return RedirectToAction(nameof(Create));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(siteWorker);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SiteWorkerExists(siteWorker.PersonID))
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
            AddClickedWorkerToSession(siteWorker);
            ViewData["IdentityUserId"] = new SelectList(_context.Users, "Id", "UserName", siteWorker.IdentityUserId);
            return View(siteWorker);
        }

        [Authorize(Roles ="Admin")]
        // GET: SiteWorkers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var siteWorker = await _context.SiteWorkers
                .Include(w => w.WorkAllocations)
                .FirstOrDefaultAsync(m => m.PersonID == id);
            if (siteWorker == null)
            {
                return NotFound();
            }

            return View(siteWorker);
        }

        [Authorize(Roles ="Admin")]
        // POST: SiteWorkers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var siteWorker = await _context.SiteWorkers
                                .Include(w => w.WorkAllocations)
                                .FirstOrDefaultAsync(w => w.PersonID == id);

            //var siteWorker = await _context.WorkAllocation
            //                    .Include(wo => wo.WorkOrder)
            //                    .FirstOrDefault(w => w.SiteWorkerPersonID == id);

            if (siteWorker.WorkAllocations.Any())
            {
                ViewData["ErrorMessage"] = "You cannot delete Site Worker with Work Order(s) assigned.";
                return View("Delete", siteWorker);
            }

            _context.SiteWorkers.Remove(siteWorker);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ManageWorkOrders(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Id = id;

            // Get the SiteWorker details
            var siteWorker = _context.SiteWorkers.FirstOrDefault(sw => sw.PersonID == id);
            if (siteWorker != null)
            {
                ViewBag.SiteWorkerName = $"{siteWorker.FirstName} {siteWorker.LastName}";
                ViewBag.SiteWorkerEmail = siteWorker.Email;
                ViewBag.HourlyWage = siteWorker.HourlyWage;
                ViewBag.StartDate = siteWorker.StartDate;
                ViewBag.IsAvailable = siteWorker.IsAvailable;
            }

            // Show ALL work orders assigned to this worker (regardless of status)
            // This allows tracking of all work orders: NotStarted, InProgress, OnHold, Completed
            var assignedWorkOrders = _context.WorkAllocation
                                      .Include(wa => wa.WorkOrder)
                                      .Include(wa => wa.WorkOrder.ProjectSite)
                                      .Include(wa => wa.SiteWorker)
                                      .Where(wa => wa.SiteWorkerPersonID == id)
                                      .OrderByDescending(wa => wa.WorkOrder.Status)  // Show InProgress first
                                      .ThenByDescending(wa => wa.AssignedDate)  // Then by assigned date
                                      .ToList();

            if (assignedWorkOrders.Count < 1)
            {
                return RedirectToAction("AddWorkOrders", new { id = id });
            }

            // Calculate statistics
            var totalWorkOrders = assignedWorkOrders.Count;
            var notStartedCount = assignedWorkOrders.Count(wo => wo.WorkOrder.Status == Models.TaskStatus.NotStarted);
            var inProgressCount = assignedWorkOrders.Count(wo => wo.WorkOrder.Status == Models.TaskStatus.InProgress);
            var completedCount = assignedWorkOrders.Count(wo => wo.WorkOrder.Status == Models.TaskStatus.Completed);
            var onHoldCount = assignedWorkOrders.Count(wo => wo.WorkOrder.Status == Models.TaskStatus.OnHold);

            // Calculate completion percentage
            var completionPercentage = totalWorkOrders > 0 ? (completedCount * 100) / totalWorkOrders : 0;

            // Calculate experience level based on completed tasks and tenure
            var yearsOfService = (DateTime.Now - siteWorker.StartDate).TotalDays / 365.25;
            var experienceScore = (completedCount * 10) + (int)(yearsOfService * 5);
            var experienceLevel = experienceScore switch
            {
                >= 100 => "Expert",
                >= 75 => "Advanced",
                >= 50 => "Intermediate",
                >= 25 => "Beginner",
                _ => "Novice"
            };

            // Calculate performance rating (0-5 stars)
            var performanceRating = completedCount > 0 ? Math.Min(5, (decimal)completedCount / 10) : 0;

            // Pass statistics to view
            ViewBag.TotalWorkOrders = totalWorkOrders;
            ViewBag.NotStartedCount = notStartedCount;
            ViewBag.InProgressCount = inProgressCount;
            ViewBag.CompletedCount = completedCount;
            ViewBag.OnHoldCount = onHoldCount;
            ViewBag.CompletionPercentage = completionPercentage;
            ViewBag.ExperienceLevel = experienceLevel;
            ViewBag.ExperienceScore = experienceScore;
            ViewBag.PerformanceRating = performanceRating;
            ViewBag.YearsOfService = Math.Round(yearsOfService, 1);

            return View(assignedWorkOrders);
        }

        public IActionResult DeleteWorkOrder(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var workAllocation = _context.WorkAllocation
                                  .Include(wa => wa.WorkOrder)
                                  .Include(wa => wa.SiteWorker)
                                  .SingleOrDefault(wa => wa.WorkID == id);

            if (workAllocation == null)
            {
                return NotFound(); 
            }

            return View(workAllocation);
        }

        [HttpPost, ActionName("DeleteWorkOrder")]
        public IActionResult DeleteWorkOrderConfirmation(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var workAlloc = _context.WorkAllocation
                              .Include(wa => wa.SiteWorker)
                              .SingleOrDefault(wa => wa.WorkID == id);

            int siteWorkerPersonID = workAlloc.SiteWorkerPersonID ?? 0;

            _context.WorkAllocation.Remove(workAlloc);
            _context.SaveChanges();

            // Check if the SiteWorker has any remaining work allocations
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

            return RedirectToAction("ManageWorkOrders", new { id = siteWorkerPersonID });
        }


        // GET Method: Many-to-Many 

        public IActionResult AddWorkOrders(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var allocatedWork = _context.WorkAllocation
                                  .Include(wa => wa.WorkOrder)
                                  .Where(wa => wa.SiteWorkerPersonID == id).ToList();

            // Only show OnHold or Pending work orders
            var allWorkOrders = _context.WorkOrders
                                  .Where(wo => wo.Status == Models.TaskStatus.OnHold || wo.Status == Models.TaskStatus.NotStarted)
                                  .ToList();

            List<WorkOrder> availableWorkOrders = new List<WorkOrder>();

            foreach (var work in allWorkOrders)
            {
                bool isAvailable = true;

                foreach (var alloc in allocatedWork)
                {
                    if (alloc.WorkOrderID == work.WorkOrderID)
                    {
                        isAvailable = false;
                    }
                }

                if (isAvailable)
                {
                    availableWorkOrders.Add(work);
                }
            }

            List<SiteWorker_AddWorkOrder_ViewModel> model = new List<SiteWorker_AddWorkOrder_ViewModel>();

            foreach (var work in availableWorkOrders)
            {
                SiteWorker_AddWorkOrder_ViewModel vm = new SiteWorker_AddWorkOrder_ViewModel();
                vm.WorkOrder = work;
                vm.Selected = false;
                model.Add(vm);
            }

            ViewBag.SiteWorkerPersonID = id;

            return View(model);
        }

        [HttpPost]
        public IActionResult AddWorkOrders(int? id, List<SiteWorker_AddWorkOrder_ViewModel> AddWorkOrders)
        {
            bool hasAssignments = false;

            foreach (var item in AddWorkOrders)
            {
                if (item.Selected)
                {
                    WorkAllocation workAlloc = new WorkAllocation();
                    workAlloc.SiteWorkerPersonID = id;
                    workAlloc.WorkOrderID = item.WorkOrder.WorkOrderID;
                    workAlloc.AssignedDate = DateTime.Now;

                    _context.WorkAllocation.Add(workAlloc);
                    hasAssignments = true;
                }
            }

            // If any work orders were assigned, update the SiteWorker's IsAvailable status to false
            if (hasAssignments)
            {
                var siteWorker = _context.SiteWorkers.FirstOrDefault(sw => sw.PersonID == id);
                if (siteWorker != null)
                {
                    siteWorker.IsAvailable = false;
                    _context.Update(siteWorker);
                }
            }

            _context.SaveChanges();
            return RedirectToAction("ManageWorkOrders", new { id = id });
        }


        private bool SiteWorkerExists(int id)
        {
            return _context.SiteWorkers.Any(e => e.PersonID == id);
        }
    }
}
