using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SatelliteSite.GroupModule.Models
{
    public class AffiliationModel
    {
        [DisplayName("ID")]
        [Required]
        public int? Id { get; set; }

        [DisplayName("Formal Name")]
        [Required]
        public string Name { get; set; }

        [DisplayName("Abbreviation (should be lowercase)")]
        [Required]
        public string Abbreviation { get; set; }

        [DisplayName("Country Code")]
        public string CountryCode { get; set; }

        [DisplayName("School Logo (size 200px)")]
        public IFormFile Logo { get; set; }
    }
}
