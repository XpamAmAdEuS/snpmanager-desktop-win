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
        public User()
        {}
        
        public User(List<string> authorities) : this()
        {
            Authorities = authorities;
        }
        

        /// <summary>
        /// Gets or sets the customer's email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the user first name.
        /// </summary>
        public string FirstName { get; set; }
        
        /// <summary>
        /// Gets or sets the user last name.
        /// </summary>
        public string LastName { get; set; }

        public string FullName { get; set; }
        
        public string ImageUrl { get; set; }
        
        public string LangKey { get; set; }
        
        public List<string> Authorities  { get; set; }
            
    }
}
