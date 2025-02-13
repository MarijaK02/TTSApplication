using System.ComponentModel.DataAnnotations;
using TTS.Domain.Enum;
using TTS.Domain.Shared;

namespace TTS.Domain.Domain
{
    public class Project : BaseEntity
    {
        [Display(Name = "Наслов")]
        public string Title { get; set; }

        [Display(Name = "Експертиза")]
        public Expertise Expertise { get; set; }

        [Display(Name = "Статус")]
        public ProjectStatus Status { get; set; }

        [Display(Name = "Опис")]
        public string? Description { get; set; }


        [Display(Name = "Датум на креирање")]
        public DateTime? CreatedOn { get; set; }

        [Display(Name = "Почетен датум")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Краен рок")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Датум на комплетирање")]
        public DateTime? CompletedOn { get; set; }

        public Guid CreatedById { get; set; }
        public Client? CreatedBy { get; set; } 
        public virtual ICollection<ConsultantProject>? ConsultantProjects { get; set; }
    }
}
