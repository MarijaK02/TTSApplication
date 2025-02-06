using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TTS.Domain.Domain;
using TTS.Domain.Identity;

namespace TTS.Repository
{
    public class ApplicationDbContext : IdentityDbContext<TTSApplicationUser>
    {
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Consultant> Consultants { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<ConsultantProject> ConsultantProjects { get; set; }
        public virtual DbSet<Attachment> Attachments { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Allow cascade delete when a Project is deleted
            modelBuilder.Entity<ConsultantProject>()
                .HasOne(cp => cp.Project)
                .WithMany(p => p.ConsultantProjects)
                .HasForeignKey(cp => cp.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);  // ✅ Allows cascading when a Project is deleted

            // Prevent cascade delete when a Consultant is deleted
            modelBuilder.Entity<ConsultantProject>()
                .HasOne(cp => cp.Consultant)
                .WithMany(c => c.Projects)
                .HasForeignKey(cp => cp.ConsultantId)
                .OnDelete(DeleteBehavior.NoAction);  // ❌ No cascade delete when a Consultant is deleted

            // Keep cascade delete for Activity → Comments
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Activity)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);  // ✅ Allows cascade delete when an Activity is deleted

            // Prevent cascade delete for User → Comments
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.CreatedBy)
                .WithMany()  // No navigation property needed
                .HasForeignKey(c => c.CreatedById)
                .OnDelete(DeleteBehavior.NoAction);
        }



    }
}
