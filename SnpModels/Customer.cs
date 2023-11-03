using System;

namespace Snp.Models
{
    /// <summary>
    /// Represents a customer.
    /// </summary>
    public class Customer : IdObject,IEquatable<Customer>
    {
        /// <summary>
        /// Gets or sets the customer's first name.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the customer's last name.
        /// </summary>
        public string Person { get; set; }

        /// <summary>
        /// Gets or sets the customer's email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the customer's phone number.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the customer's address. 
        /// </summary>
        public string Address { get; set; }
        
        public bool Muted { get; set; }
        
        public uint SizeLimit { get; set; }

        /// <summary>
        /// Returns the customer's title.
        /// </summary>
        public override string ToString() =>  $"{Id} {Title} {Email}";

        public bool Equals(Customer other) =>
            Id == other.Id &&
            Title == other.Title &&
            Person == other.Person &&
            Email == other.Email &&
            Phone == other.Phone &&
            Address == other.Address;

        public Customer FromPb(Snp.V1.Customer pb)
        {
            Id = pb.Id;
            Title = pb.Title;
            Person = pb.Person;
            Email = pb.Email;
            Phone = pb.Phone;
            Address = pb.Address;
            Muted = pb.Muted;
            SizeLimit = pb.SizeLimit;
            return this;

        }
            
    }
}
