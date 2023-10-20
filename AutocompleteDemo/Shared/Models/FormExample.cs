using System.ComponentModel.DataAnnotations;

namespace AutocompleteDemo.Shared.Models
{
    /// <summary>
    /// Class FormExample.
    /// </summary>
    public class FormExample
    {
        /// <summary>
        /// Gets or sets the selected person.
        /// </summary>
        /// <value>The selected person.</value>
        [Required]
        public Person SelectedPerson { get; set; }
    }
}