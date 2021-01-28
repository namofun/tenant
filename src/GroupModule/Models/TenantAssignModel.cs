using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SatelliteSite.GroupModule.Models
{
    public class TenantAssignModel
    {
        [DisplayName("User Name")]
        [Required]
        public string UserName { get; set; }
    }
}
