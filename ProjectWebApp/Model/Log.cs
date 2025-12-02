using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    public partial class Log
    {
        [Key]
        public int LogId { get; set; }
        public int? UserId { get; set; }
        [Required]
        [StringLength(50)]
        public string ActionType { get; set; }
        [Required]
        [StringLength(500)]
        public string Description { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime Timestamp { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("Logs")]
        public virtual UserProfile User { get; set; }
    }
}
