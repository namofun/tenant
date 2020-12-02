using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Tenant.Entities;

namespace SatelliteSite.GroupModule.Models
{
    public class CreateTeamModel
    {
        [DisplayName("Team name")]
        [Required]
        public string TeamName { get; set; }

        [DisplayName("Affiliation")]
        public int AffiliationId { get; set; }

        public ICollection<Affiliation> Affiliations { get; set; }
    }
}
