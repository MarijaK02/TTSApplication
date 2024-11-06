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
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
