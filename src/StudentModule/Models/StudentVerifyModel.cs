using System.ComponentModel.DataAnnotations;

namespace SatelliteSite.StudentModule.Models
{
    public class StudentVerifyModel
    {
        [Display(Name = "Student ID")]
        [Required]
        public string StudentId { get; set; }

        [Display(Name = "Affiliation")]
        [Required]
        public int AffiliationId { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Student Name")]
        [Required]
        public string StudentName { get; set; }

        public bool IsEmailConfirmed { get; set; }

        [EmailAddress]
        [Display(Name = "Student Email")]
        [Required]
        public string Email { get; set; }
    }
}
