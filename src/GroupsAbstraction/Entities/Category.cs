﻿namespace Tenant.Entities
{
    /// <summary>
    /// The entity class for team categories.
    /// </summary>
    public class Category
    {
        /// <summary>
        /// The category ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The category name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The category color
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// The sort order
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// Whether to show to public
        /// </summary>
        public bool IsPublic { get; set; }

#pragma warning disable CS8618
        /// <summary>
        /// Initialize a category entity.
        /// </summary>
        public Category()
        {
        }
#pragma warning restore CS8618
    }
}
