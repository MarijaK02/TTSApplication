using System.ComponentModel.DataAnnotations;

namespace AdminApplication.Models.Enums
{
    public enum ApplicationStatus
    {
        [Display(Name = "Испратена")]
        Applied,

        [Display(Name = "Прифатена")]
        Accepted,

        [Display(Name = "Одбиена")]
        Rejected
    }
}
