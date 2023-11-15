using System;

namespace SnpApp.Models
{
    public class Site : IdObject
    {
        
        
        public Site(){}
        
        public Site(Customer customer) : this()
        {
            Customer = customer;
            CustomerId = customer.Id;
        }
        
        public Customer Customer { get; set; }
        
        public uint CustomerId { get; }
        
        public string Code { get; set; }
        
        public bool FtpStatsEnabled { get; set; }
        public bool SeekIntoEnabled { get; set; }
      public string Notes  { get; set; }
      
      public int CrossFade  { get; set; }
      public bool  FtpStatsDisabled { get; set; }
      public DateTime Expiration { get; set; }
      public bool  Disabled { get; set; }
      public int ReconnectionPeriod { get; set; }
      public string Status { get; set; }
      public string Description { get; set; }
      public int Volume  { get; set; }
      public string PlaylistType { get; set; }
      public string OpenMon { get; set; }
      public string CloseMon { get; set; }
      public string OpenTue { get; set; } 
      public string CloseTue { get; set; }
      public string OpenWed { get; set; }
      public string CloseWed { get; set; }
      public string OpenThu { get; set; }
      public string CloseThu { get; set; }
      public string OpenFri { get; set; }
      public string CloseFri { get; set; }
      public string OpenSat { get; set; }
      public string CloseSat { get; set; }
      public string OpenSun { get; set; }
      public string CloseSun { get; set; }
        
      
        public string Title { get; set; }

        
        public string Person { get; set; }

       
        public string Email { get; set; }

     
        public string Phone { get; set; }

      
        public string Address { get; set; }
        
        public bool Muted { get; set; }
        
        public uint SizeLimit { get; set; }
            
    }
}
