using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    [Index("Code", Name = "UQ__Airports__A25C5AA758DE3086", IsUnique = true)]
    public partial class Airport
    {
        public Airport()
        {
            FlightDestinationAirports = new HashSet<Flight>();
            FlightOriginAirports = new HashSet<Flight>();
            UserPreferences = new HashSet<UserPreference>();
        }

        [Key]
        public int AirportId { get; set; }
        [Required]
        [StringLength(100)]
        public string AirportName { get; set; }
        [Required]
        [StringLength(10)]
        public string Code { get; set; }
        [Required]
        [StringLength(100)]
        public string City { get; set; }
        public int CountryId { get; set; }

        [ForeignKey("CountryId")]
        [InverseProperty("Airports")]
        public virtual Country Country { get; set; }
        [InverseProperty("DestinationAirport")]
        public virtual ICollection<Flight> FlightDestinationAirports { get; set; }
        [InverseProperty("OriginAirport")]
        public virtual ICollection<Flight> FlightOriginAirports { get; set; }
        [InverseProperty("PreferredOriginAirport")]
        public virtual ICollection<UserPreference> UserPreferences { get; set; }
    }
}
