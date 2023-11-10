using System;

namespace Snp.Core.Models
{
    /// <summary>
    /// Represents a site.
    /// </summary>
    public class Site : IdObject,IEquatable<Site>
    {
        
        /// <summary>
        /// Creates a new site.
        /// </summary>
        public Site()
        { }
        
        /// <summary>
        /// Creates a new order for the given customer.
        /// </summary>
        public Site(Customer customer) : this()
        {
            Customer = customer;
            CustomerId = customer.Id;
        }
        
        /// <summary>
        /// Gets or sets the customer placing the order.
        /// </summary>
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
      // public SitePlayer SitePlayer { get; set; }
      // public SiteAnnouncement SiteAnnouncement { get; set; }
      
      // public string OpenMonShort => OpenMon.ToString("t");
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

        public bool Equals(Site other) =>
            Id == other.Id &&
            Title == other.Title &&
            Person == other.Person &&
            Email == other.Email &&
            Phone == other.Phone &&
            Address == other.Address;
            
    }
}
