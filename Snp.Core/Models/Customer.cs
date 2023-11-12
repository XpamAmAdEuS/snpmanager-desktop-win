using System;
using System.Collections.Generic;

namespace Snp.Core.Models
{
    
    public class Customer : IdObject,IEquatable<Customer>
    {
      
        public string Title { get; set; }
        
        public string Person { get; set; }
     
        public string Email { get; set; }
        
        public string Phone { get; set; }
        
        public string Address { get; set; }
        
        public bool Muted { get; set; }
        
        public SizeLimitModel SizeLimit { get; set; }
        
        public ulong SizeLimitValue { get; set; }
        
        public List<Site> Sites  { get; set; }

        public bool Equals(Customer other) =>
            Id == other.Id &&
            Title == other.Title &&
            Person == other.Person &&
            Email == other.Email &&
            Phone == other.Phone &&
            Address == other.Address;
            
    }
}
