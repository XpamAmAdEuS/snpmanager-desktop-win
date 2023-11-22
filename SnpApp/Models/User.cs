using System.Collections.Generic;

namespace SnpApp.Models
{
    /// <summary>
    /// Represents a site.
    /// </summary>
    public class User : IdObject
    {
        
        /// <summary>
        /// Creates a new site.
        /// </summary>
        public User(){}
        
        public User(List<string> authorities) : this()
        {
            Authorities = authorities;
        }
        

        /// <summary>
        /// Gets or sets the customer's email.
        /// </summary>
        public string Email { get; set; }  = default!;

        /// <summary>
        /// Gets or sets the user first name.
        /// </summary>
        public string FirstName { get; set; } = default!;
        
        /// <summary>
        /// Gets or sets the user last name.
        /// </summary>
        public string LastName { get; set; } = default!;

        public string FullName { get; set; } = default!;
        
        public string ImageUrl { get; set; } = default!;
        
        public string LangKey { get; set; } = default!;
        
        public List<string> Authorities  { get; set; } = default!;
            
    }
}
