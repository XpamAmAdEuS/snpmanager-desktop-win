using System;
using System.Collections.Generic;

namespace SnpCore.Models
{
    /// <summary>
    /// Represents a site.
    /// </summary>
    public class User : IEquatable<User>
    {
        
        /// <summary>
        /// Creates a new site.
        /// </summary>
        public User()
        { }
        
        public string Id  { get; set; }
        
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

        /// <summary>
        /// Returns the customer's title.
        /// </summary>
        public override string ToString() =>  $"{Id} {FullName} {Email}";

        public bool Equals(User other) =>
            Id == other.Id &&
            FirstName == other.FullName &&
            LastName == other.LastName &&
            FullName == other.FullName &&
            Email == other.Email &&
            LangKey == other.LangKey &&
            ImageUrl == other.ImageUrl &&
            Authorities == other.Authorities;
            
    }
}
