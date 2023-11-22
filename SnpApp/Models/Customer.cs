using System.Collections.Generic;

namespace SnpApp.Models
{
    
    public class Customer : IdObject
    {
      
        public string Title { get; set; } = default!;
        
        public string Person { get; set; } = default!;
     
        public string Email { get; set; } = default!;
        
        public string Phone { get; set; } = default!;
        
        public string Address { get; set; } = default!;
        
        public bool Muted { get; set; }
        
        public SizeLimitModel SizeLimit { get; set; } = default!;
        
        public ulong SizeLimitValue { get; set; }
        
        public List<Site> Sites  { get; set; } = default!;
    }
}
