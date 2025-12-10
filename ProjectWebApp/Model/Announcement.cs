using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    public partial class Announcement
    {
        [Key]
        public int AnnouncementId { get; set; }
        [Required]
        [StringLength(150)]
        public string Title { get; set; }
        [Required]
        public string Message { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        public int? UserId { get; set; }
        public int? CreatedBy { get; set; }

        [ForeignKey("CreatedBy")]
        [InverseProperty("AnnouncementCreatedByNavigations")]
        public virtual UserProfile CreatedByNavigation { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("AnnouncementUsers")]
        public virtual UserProfile User { get; set; }
    }
}
