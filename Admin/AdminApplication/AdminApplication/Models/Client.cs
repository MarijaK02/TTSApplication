using AdminApplication.Models.Enums;
using Microsoft.CodeAnalysis;

namespace AdminApplication.Models
{
    public class Client
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public TTSApplicationUser User { get; set; }

        public string? Address { get; set; }
        public Industry Industry { get; set; }

        public ICollection<Project>? Projects { get; set; }
    }
}
