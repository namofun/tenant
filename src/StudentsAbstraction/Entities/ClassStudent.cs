namespace Tenant.Entities
{
    /// <summary>
    /// The entity class for relation between classes and students.
    /// </summary>
    public class ClassStudent
    {
        /// <summary>
        /// The class ID
        /// </summary>
        public int ClassId { get; set; }

        /// <summary>
        /// The student ID
        /// </summary>
        public string StudentId { get; set; }

#pragma warning disable CS8618
        /// <summary>
        /// Instantiate a pair of class and student.
        /// </summary>
        public ClassStudent()
        {
        }
#pragma warning restore CS8618
    }
}
