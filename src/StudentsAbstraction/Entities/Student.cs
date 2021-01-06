namespace Tenant.Entities
{
    /// <summary>
    /// The entity class for students.
    /// </summary>
    public class Student
    {
        /// <summary>
        /// The student ID
        /// </summary>
        /// <remarks>Usually constructed as strings like <c>10183_55171102</c>.</remarks>
        public string Id { get; set; }

        /// <summary>
        /// The affiliation ID
        /// </summary>
        public int AffiliationId { get; set; }

        /// <summary>
        /// The student name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// [Ignore] The corresponding student name
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// [Ignore] The corresponding student email
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// [Ignore] The corresponding user ID
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        /// [Ignore] Whether student has been verified
        /// </summary>
        public bool? IsVerified { get; set; }

#pragma warning disable CS8618
        /// <summary>
        /// Initialize a student entity.
        /// </summary>
        public Student()
        {
        }
#pragma warning restore CS8618
    }
}
