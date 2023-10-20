using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutocompleteDemo.Shared.Models
{
    /// <summary>
    /// Class Tag.
    /// </summary>
    public class Tag
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Tag"/> class.
        /// </summary>
        public Tag() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tag"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="tagDescription">The tag description.</param>
        /// <param name="tagCount">The tag count.</param>
        public Tag(int id, string tagName, string tagDescription, int tagCount)
        {
            Id = id;
            TagName = tagName;
            TagDescription = tagDescription;
            TagCount = tagCount;
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the name of the tag.
        /// </summary>
        /// <value>The name of the tag.</value>
        public string TagName { get; set; }
        /// <summary>
        /// Gets or sets the tag description.
        /// </summary>
        /// <value>The tag description.</value>
        public string TagDescription { get; set; }
        /// <summary>
        /// Gets or sets the tag count.
        /// </summary>
        /// <value>The tag count.</value>
        public int TagCount { get; set; }
    }
}
