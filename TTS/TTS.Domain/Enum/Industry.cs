using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTS.Domain.Enum
{
    public enum Industry
    {
        [Display(Name = "E-Commerce")]
        ECommerce,

        [Display(Name = "Financial Technology (FinTech)")]
        FinTech,

        [Display(Name = "Health Technology (HealthTech)")]
        HealthTech,

        [Display(Name = "Education Technology (EdTech)")]
        EdTech,

        [Display(Name = "Gaming Industry")]
        Gaming,

        [Display(Name = "Real Estate")]
        RealEstate,

        [Display(Name = "Travel and Tourism")]
        Travel,

        [Display(Name = "Logistics and Supply Chain")]
        Logistics,

        [Display(Name = "Entertainment Industry")]
        Entertainment,

        [Display(Name = "Cybersecurity")]
        Cybersecurity,

        [Display(Name = "Artificial Intelligence (AI)")]
        ArtificialIntelligence,

        [Display(Name = "Data Analytics")]
        DataAnalytics,

        [Display(Name = "Telecommunications")]
        Telecommunications,

        [Display(Name = "Blockchain Technology")]
        Blockchain,

        [Display(Name = "Cloud Computing")]
        CloudComputing,

        [Display(Name = "Marketing and Advertising")]
        Marketing,

        [Display(Name = "Manufacturing")]
        Manufacturing,

        [Display(Name = "Retail")]
        Retail,

        [Display(Name = "Government Services")]
        Government,

        [Display(Name = "Non-Profit Organizations")]
        NonProfit
    }
}
