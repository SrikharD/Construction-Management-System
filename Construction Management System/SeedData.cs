// Created By: Srikhar Dogiparthy

using Dogiparthy_Spring25.Data;
using Dogiparthy_Spring25.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dogiparthy_Spring25
{
    public class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await EnsureRolesAsync(roleManager);
            var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
            var context = services.GetRequiredService<ApplicationDbContext>();

            await EnsureTestAdminAsync(userManager);
            await EnsureTestStandardUserAsync(userManager, context);

        }
        public static async Task EnsureRolesAsync(RoleManager<IdentityRole>
        roleManager)
        {
            var adminAlreadyExists = await roleManager.RoleExistsAsync
            (Constants.AdminRole);
            if (adminAlreadyExists)
            {
                return;
            }
            await roleManager.CreateAsync(new IdentityRole(Constants.AdminRole));
            var stdUserAlreadyExists = await roleManager.RoleExistsAsync
            (Constants.StandardUserRole);
            if (stdUserAlreadyExists)
            {
                return;
            }
            await roleManager.CreateAsync(new IdentityRole(Constants.StandardUserRole));
        }
        public static async Task EnsureTestAdminAsync
        (UserManager<IdentityUser> userManager)
        {
            var testAdmin = await userManager.Users.Where(x => x.UserName
            == "admin@admin.com").SingleOrDefaultAsync();
            if (testAdmin != null)
            {
                return;
            }
            testAdmin = new IdentityUser
            {
                UserName = "Administrator",
                Email = "admin@admin.com"
            };
            await userManager.CreateAsync(testAdmin, "Password1!");
            await userManager.AddToRoleAsync(testAdmin, Constants.AdminRole);
        }

        public static async Task EnsureTestStandardUserAsync(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            var stdUser = await userManager.Users.FirstOrDefaultAsync(x => x.Email == "user@demo.com");
            if (stdUser == null)
            {
                // Check if a Person without IdentityUserId exists
                var person = context.People.FirstOrDefault(p => string.IsNullOrEmpty(p.IdentityUserId));
                if (person == null)
                {
                    // Create new dummy person
                    person = new Person
                    {
                        FirstName = "Demo",
                        LastName = "User",
                        Email = "user@demo.com",
                        PhoneNumber = "1234567890",
                        Address1 = "123 Demo Street",
                        City = "Sample City",
                        State = "AL",
                        ZipCode = "99999",
                        DOB = DateTime.Now.AddYears(-25)
                    };
                    context.People.Add(person);
                    await context.SaveChangesAsync();
                }

                stdUser = new IdentityUser
                {
                    UserName = "DemoUser",
                    Email = person.Email
                };

                var result = await userManager.CreateAsync(stdUser, "Password1!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(stdUser, Constants.StandardUserRole);

                    person.IdentityUserId = stdUser.Id;
                    context.People.Update(person);
                    await context.SaveChangesAsync();
                }
            }
        }

    }
}
