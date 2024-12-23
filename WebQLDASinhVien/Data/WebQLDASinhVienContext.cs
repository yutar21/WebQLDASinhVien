using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebQLDASinhVien.Models;

namespace WebQLDASinhVien.Data
{
    public class WebQLDASinhVienContext : IdentityDbContext<ApplicationUser>
    {
        public WebQLDASinhVienContext(DbContextOptions<WebQLDASinhVienContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Thiết lập mối quan hệ
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Student)
                .WithMany(s => s.Projects)
                .HasForeignKey(p => p.StudentId);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Teacher)
                .WithMany(t => t.Projects)
                .HasForeignKey(p => p.TeacherId);
        }
    }
}
