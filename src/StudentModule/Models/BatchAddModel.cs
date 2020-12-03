using System.ComponentModel.DataAnnotations;

namespace SatelliteSite.StudentModule.Models
{
    public class BatchAddModel
    {
        [Required]
        public string Batch { get; set; }
    }
}
