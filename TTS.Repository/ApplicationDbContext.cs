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
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Activity> Activities { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<ConsultantWorksOnProject> ConsultantWorksOnProjects { get; set; }
        public virtual DbSet<Attachment> Attachments { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
