using System.Collections.Generic;

namespace SnpCore.Models
{
    
    public class Customer : IdObject
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
    }
}
