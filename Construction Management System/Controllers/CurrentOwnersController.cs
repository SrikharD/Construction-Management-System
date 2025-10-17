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
using Microsoft.AspNetCore.Http;
using Dogiparthy_Spring25.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Build.Evaluation;
using Microsoft.AspNetCore.Identity;

namespace Dogiparthy_Spring25.Controllers
{
    [Authorize]
    public class CurrentOwnersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CurrentOwnersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin,StandardUser")]

        // GET: CurrentOwners
        public async Task<IActionResult> Index()
        {
            var sessionClicks = HttpContext.Session.Get<List<CurrentOwner>>("UserOwnerClicks");

            if (sessionClicks != null)
            {
                ViewBag.UserOwnerClicks = sessionClicks;
            }

            IQueryable<CurrentOwner> query = _context.CurrentOwners.Include(c => c.IdentityUser);

            // Role-based filtering
            if (User.IsInRole("StandardUser"))
            {
                // StandardUser (CurrentOwner) can only see themselves
                var userId = _userManager.GetUserId(User);
                query = query.Where(co => co.IdentityUserId == userId);
            }
            // Admin can see all

            var applicationDbContext = query;
            return View(await applicationDbContext.ToListAsync());
        }

        [Authorize(Roles = "Admin,StandardUser")]


        // GET: CurrentOwners/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentOwner = await _context.CurrentOwners
                .Include(c => c.IdentityUser)
                .FirstOrDefaultAsync(m => m.PersonID == id);

            if (currentOwner == null)
            {
                return NotFound();
            }

            // Role-based access control
            if (User.IsInRole("StandardUser"))
            {
                var userId = _userManager.GetUserId(User);
                if (currentOwner.IdentityUserId != userId)
                {
                    return Forbid(); // User can only view their own details
                }
            }

            AddClickedOwnerToSession(currentOwner); // Track clicks
            return View(currentOwner);
        }

        [HttpPost]
        public IActionResult GetOwnerProjectSites(int id)
        {
            var sites = _context.ProjectSites
                        .Where(e => e.CurrentOwnerPersonID == id).ToList();
            return PartialView("_PartialOwnerProjectSites", sites);
        }


        private void AddClickedOwnerToSession(CurrentOwner owner)
        {
            var sessionClicks = HttpContext.Session.Get<List<CurrentOwner>>("UserOwnerClicks");

            if (sessionClicks == null)
            {
                sessionClicks = new List<CurrentOwner>();
            }

            var ownerInSession = sessionClicks.FirstOrDefault(o => o.PersonID == owner.PersonID);

            if (ownerInSession == null)
            {
                sessionClicks.Add(owner);
                HttpContext.Session.Set<List<CurrentOwner>>("UserOwnerClicks", sessionClicks);
            }
        }

        [Authorize(Roles ="Admin")]
        // GET: CurrentOwners/Create
        public IActionResult Create()
        {
            var sessionClicks = HttpContext.Session.Get<List<CurrentOwner>>("UserOwnerClicks");
            if (sessionClicks != null)
            {
                ViewBag.UserOwnerClicks = sessionClicks;
            }

            return View();
        }


        [Authorize(Roles ="Admin")]
        // POST: CurrentOwners/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Occupation,PreferredContactMethod,PreferredPaymentMethod,PersonID,FirstName,LastName,Email,PhoneNumber,Address1,Address2,City,State,ZipCode,DOB")] CurrentOwner currentOwner)
        {
            if (currentOwner.DOB > DateTime.Today)
            {
                TempData["DOBError"] = "Date of Birth cannot be in the future.";
                return View(currentOwner);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Create a new StandardUser account for this CurrentOwner
                    var newUser = new IdentityUser
                    {
                        UserName = currentOwner.Email,
                        Email = currentOwner.Email,
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

                        // Link the user to the CurrentOwner
                        currentOwner.IdentityUserId = newUser.Id;

                        // Add the CurrentOwner to the database
                        _context.Add(currentOwner);
                        await _context.SaveChangesAsync();

                        AddClickedOwnerToSession(currentOwner);

                        TempData["SuccessMessage"] = $"Current Owner created successfully! A StandardUser account has been created with username: {newUser.UserName}. Password: Password1!";
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

            return View(currentOwner);
        }

        [Authorize(Roles ="Admin")]
        // GET: CurrentOwners/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentOwner = await _context.CurrentOwners.FindAsync(id);
            if (currentOwner == null)
            {
                return NotFound();
            }

            AddClickedOwnerToSession(currentOwner); // Track click for edit
            return View(currentOwner);
        }

        [Authorize(Roles ="Admin")]
        // POST: CurrentOwners/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Occupation,PreferredContactMethod,PreferredPaymentMethod,PersonID,FirstName,LastName,Email,PhoneNumber,Address1,Address2,City,State,ZipCode,DOB")] CurrentOwner currentOwner)
        {
            if (id != currentOwner.PersonID)
            {
                return NotFound();
            }
            if (currentOwner.DOB > DateTime.Today)
            {
                TempData["DOBError"] = "Date of Birth cannot be in the future.";
                return View(currentOwner);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get the existing owner to preserve the IdentityUserId
                    var existingOwner = await _context.CurrentOwners.FindAsync(id);
                    if (existingOwner != null)
                    {
                        currentOwner.IdentityUserId = existingOwner.IdentityUserId;
                    }

                    _context.Update(currentOwner);
                    await _context.SaveChangesAsync();
                    AddClickedOwnerToSession(currentOwner); // Track click after edit
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CurrentOwnerExists(currentOwner.PersonID))
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
            return View(currentOwner);
        }

        [Authorize(Roles ="Admin")]
        // GET: CurrentOwners/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var currentOwner = await _context.CurrentOwners
                .Include(x => x.Properties)   
                .Include(c => c.IdentityUser)
                .FirstOrDefaultAsync(m => m.PersonID == id);
            if (currentOwner == null)
            {
                return NotFound();
            }

            AddClickedOwnerToSession(currentOwner); // Track click before delete
            return View(currentOwner);
        }

        [Authorize(Roles ="Admin")]
        // POST: CurrentOwners/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var currentOwner = await _context.CurrentOwners
                .Include(x => x.Properties)
                .Include(c => c.IdentityUser)
                .FirstOrDefaultAsync(m => m.PersonID == id);

            //if (currentOwner != null)
            //{
            //    ViewData["ErrorMessage"] = "You cannot delete Owner with Property(s) assigned.";

            //    return View("Delete", currentOwner);
            //}
            _context.CurrentOwners.Remove(currentOwner);
             AddClickedOwnerToSession(currentOwner); // Track click after deletion
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

       


        private bool CurrentOwnerExists(int id)
        {
            return _context.CurrentOwners.Any(e => e.PersonID == id);
        }

        /// <summary>
        /// Generates a temporary password that meets ASP.NET Identity requirements
        /// Requirements: At least 6 characters, 1 uppercase, 1 lowercase, 1 digit, 1 special character
        /// </summary>
        private string GenerateTemporaryPassword()
        {
            const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowercase = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string special = "!@#$%^&*";

            var random = new Random();
            var password = new System.Text.StringBuilder();

            // Add one of each required character type
            password.Append(uppercase[random.Next(uppercase.Length)]);
            password.Append(lowercase[random.Next(lowercase.Length)]);
            password.Append(digits[random.Next(digits.Length)]);
            password.Append(special[random.Next(special.Length)]);

            // Add random characters to reach minimum length
            const string allChars = uppercase + lowercase + digits + special;
            for (int i = password.Length; i < 10; i++)
            {
                password.Append(allChars[random.Next(allChars.Length)]);
            }

            // Shuffle the password
            var passwordArray = password.ToString().ToCharArray();
            for (int i = passwordArray.Length - 1; i > 0; i--)
            {
                int randomIndex = random.Next(i + 1);
                var temp = passwordArray[i];
                passwordArray[i] = passwordArray[randomIndex];
                passwordArray[randomIndex] = temp;
            }

            return new string(passwordArray);
        }
    }
}
