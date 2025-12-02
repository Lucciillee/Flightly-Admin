using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    public partial class FeaturedDestination
    {
        [Key]
        public int DestinationId { get; set; }
        [Required]
        [StringLength(150)]
        public string CityName { get; set; }
        [StringLength(150)]
        public string Country { get; set; }
        [StringLength(500)]
        public string ImageUrl { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        public int? UserId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DeletedAt { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("FeaturedDestinations")]
        public virtual UserProfile User { get; set; }
    }
}
