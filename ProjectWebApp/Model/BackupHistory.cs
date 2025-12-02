using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebApp.Model
{
    [Table("BackupHistory")]
    public partial class BackupHistory
    {
        [Key]
        public int BackupId { get; set; }
        [Required]
        [StringLength(255)]
        public string FileName { get; set; }
        [Required]
        [StringLength(500)]
        public string FilePath { get; set; }
        [StringLength(50)]
        public string BackupType { get; set; }
        [StringLength(50)]
        public string BackupStatus { get; set; }
        [StringLength(500)]
        public string Notes { get; set; }
        public int? UserId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [StringLength(50)]
        public string RestoreStatus { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RestoreTimestamp { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("BackupHistories")]
        public virtual UserProfile User { get; set; }
    }
}
