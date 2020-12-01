using System.Collections.Generic;

namespace Tenant.Entities
{
    /// <summary>
    /// The entity class for teaching class.
    /// </summary>
    public class Class
    {
        /// <summary>
        /// The teaching class ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The affiliation ID
        /// </summary>
        public int AffiliationId { get; set; }

        /// <summary>
        /// The teaching class name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// [Ignore] The count of students
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// The navigation to students
        /// </summary>
        public ICollection<ClassStudent> Students { get; set; }

        /// <summary>
        /// The navigation to affiliation
        /// </summary>
        public Affiliation Affiliation { get; set; }

#pragma warning disable CS8618
        /// <summary>
        /// Initialize a teaching class entity.
        /// </summary>
        public Class()
        {
        }
#pragma warning restore CS8618
    }
}
