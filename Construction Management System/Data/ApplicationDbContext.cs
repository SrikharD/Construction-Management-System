// Created By: Srikhar Dogiparthy

using Dogiparthy_Spring25.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;

namespace Dogiparthy_Spring25.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Person> People { get; set; }
        public DbSet<CurrentOwner> CurrentOwners { get; set; }

        public DbSet<SiteWorker> SiteWorkers { get; set; }

        public DbSet<ProjectSite> ProjectSites { get; set; }

        public DbSet<WorkOrder> WorkOrders { get; set; }

        public DbSet<WorkAllocation> WorkAllocation { get; set; }


        /* for one-to-one relationship
            between user and CurrentOwner
         */

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Person>()
                .HasOne(co => co.IdentityUser)
                .WithOne()
                .HasForeignKey<Person>(co => co.IdentityUserId);

            // Configure decimal precision for HourlyWage
            builder.Entity<SiteWorker>()
                .Property(sw => sw.HourlyWage)
                .HasPrecision(10, 2);
        }

    }
}
