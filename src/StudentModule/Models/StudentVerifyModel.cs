using System.ComponentModel.DataAnnotations;

namespace SatelliteSite.StudentModule.Models
{
    public class StudentVerifyModel
    {
        [Display(Name = "Student ID")]
        public string StudentId { get; set; }

        [Display(Name = "Affiliation")]
        [Required]
        public int AffiliationId { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Student Name")]
        [Required]
        public string StudentName { get; set; }

        public bool? PendingConfirm { get; set; }

        [EmailAddress]
        [Display(Name = "Student Email")]
        public string Email { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(10)]
        [Display(Name = "Verification Code")]
        public string VerifyCode { get; set; }

        [Display(Name = "Verification type")]
        public int VerifyOption { get; set; }
    }
}
