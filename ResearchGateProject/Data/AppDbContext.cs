using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ResearchGateProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ResearchGateProject.Data
{
    public class AppDbContext: IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser_Paper>().HasKey(am => new
            {
                am.ApplicationUserId,
                am.PaperId
            });

            modelBuilder.Entity<ApplicationUser_Paper>().HasOne(m => m.ApplicationUser).WithMany(am => am.ApplicationUsers_Papers).HasForeignKey(m => m.ApplicationUserId);
            modelBuilder.Entity<ApplicationUser_Paper>().HasOne(m => m.Paper).WithMany(am => am.ApplicationUsers_Papers).HasForeignKey(m => m.PaperId);

            base.OnModelCreating(modelBuilder);
        }


       
        public DbSet<ApplicationUser_Paper> ApplicationUsers_Papers { get; set; }
        public DbSet<Paper> Papers { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Comment> Comments { get; set; }



    }
}
