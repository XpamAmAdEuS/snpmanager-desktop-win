using System;

namespace Snp.Models
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
      public TimeOnly OpenMon { get; set; }
      public TimeOnly CloseMon { get; set; }
      public TimeOnly OpenTue { get; set; } 
      public TimeOnly CloseTue { get; set; }
      public TimeOnly OpenWed { get; set; }
      public TimeOnly CloseWed { get; set; }
      public TimeOnly OpenThu { get; set; }
      public TimeOnly CloseThu { get; set; }
      public TimeOnly OpenFri { get; set; }
      public TimeOnly CloseFri { get; set; }
      public TimeOnly OpenSat { get; set; }
      public TimeOnly CloseSat { get; set; }
      public TimeOnly OpenSun { get; set; }
      public TimeOnly CloseSun { get; set; }
        
        
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

        public Site FromPb(Snp.V1.Site pb)
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
