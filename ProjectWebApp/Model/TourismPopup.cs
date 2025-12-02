using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    [Table("TourismPopup")]
    public partial class TourismPopup
    {
        [Key]
        public int PopupId { get; set; }
        [Required]
        [StringLength(100)]
        public string PlaceName { get; set; }
        [Required]
        public string PopupText { get; set; }
        [StringLength(500)]
        public string ImageUrl { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }
        public int? UserId { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("TourismPopups")]
        public virtual UserProfile User { get; set; }
    }
}
