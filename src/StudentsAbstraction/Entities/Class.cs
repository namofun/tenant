using System;
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
        /// The creation time of class
        /// </summary>
        public DateTimeOffset CreationTime { get; set; }

        /// <summary>
        /// The user ID of creator
        /// </summary>
        /// <remarks><c>null</c> when this is publicly visible.</remarks>
        public int? UserId { get; set; }

        /// <summary>
        /// The user name of creator
        /// </summary>
        public string? UserName { get; set; }

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
