using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    [Table("TimeZone")]
    public class TimeZone
    {
        [Key]
        public int TimeZoneSN { get; set; } // 序號，自動遞增

        [Required]
        public int TenantSN { get; set; } // 所屬養殖戶 SN

        [Required]
        [StringLength(20)]
        public string TimeZoneNum { get; set; } = string.Empty; // 時間區段編碼

        [Required]
        [StringLength(50)]
        public string TimeZoneDesc { get; set; } = string.Empty; // 時間區段描述

        [StringLength(100)]
        public string? Remark { get; set; } // 備註

        [Required]
        public bool IsDeleted { get; set; } = false; // 是否刪除：0=否，1=是

        [Required]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string CreateUser { get; set; } = string.Empty;

        public DateTime? ModifyTime { get; set; }

        [StringLength(50)]
        public string? ModifyUser { get; set; }

        public DateTime? DeleteTime { get; set; }

        [StringLength(50)]
        public string? DeleteUser { get; set; }

        public ICollection<FeedingRecord> FeedingRecords { get; set; } = new List<FeedingRecord>();
    }
}