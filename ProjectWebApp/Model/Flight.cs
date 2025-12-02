using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    [Index("FlightNumber", Name = "UQ__Flights__2EAE6F507D19C108", IsUnique = true)]
    public partial class Flight
    {
        public Flight()
        {
            Bookings = new HashSet<Booking>();
        }

        [Key]
        public int FlightId { get; set; }
        public int AirlineId { get; set; }
        [Required]
        [StringLength(20)]
        public string FlightNumber { get; set; }
        [StringLength(50)]
        public string Aircraft { get; set; }
        public int OriginAirportId { get; set; }
        public int DestinationAirportId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DepartureTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ArrivalTime { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal BasePrice { get; set; }
        public int StatusId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        public int? UserId { get; set; }

        [ForeignKey("AirlineId")]
        [InverseProperty("Flights")]
        public virtual Airline Airline { get; set; }
        [ForeignKey("DestinationAirportId")]
        [InverseProperty("FlightDestinationAirports")]
        public virtual Airport DestinationAirport { get; set; }
        [ForeignKey("OriginAirportId")]
        [InverseProperty("FlightOriginAirports")]
        public virtual Airport OriginAirport { get; set; }
        [ForeignKey("StatusId")]
        [InverseProperty("Flights")]
        public virtual FlightStatus Status { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("Flights")]
        public virtual UserProfile User { get; set; }
        [InverseProperty("Flight")]
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
