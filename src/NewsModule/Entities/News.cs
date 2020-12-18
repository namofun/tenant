using System;

namespace SatelliteSite.NewsModule.Entities
{
    /// <summary>
    /// The entity class for news.
    /// </summary>
    public class News
    {
        /// <summary>
        /// The news ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Whether to show to public
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// The news title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The last update time
        /// </summary>
        public DateTimeOffset LastUpdate { get; set; }

        /// <summary>
        /// The markdown source code
        /// </summary>
        public byte[] Source { get; set; }

        /// <summary>
        /// The HTML content
        /// </summary>
        public byte[] Content { get; set; }

        /// <summary>
        /// The semantics tree in HTML
        /// </summary>
        public byte[] Tree { get; set; }
    }
}
