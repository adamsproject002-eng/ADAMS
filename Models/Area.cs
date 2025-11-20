using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    [Table("Area")]
    public class Area
    {
        [Key]
        public int AreaSN { get; set; } // 序號，自動增加

        [Required]
        [StringLength(50)]
        public string AreaName { get; set; } = string.Empty; // 場區編號

        [Required]
        public int TenantSN { get; set; } // 所屬養殖戶SN

        [StringLength(20)]
        public string? GPSX { get; set; } // 養殖場區代表GPS座標(經度X)

        [StringLength(20)]
        public string? GPSY { get; set; } // 養殖場區代表GPS座標(緯度Y)

        [StringLength(100)]
        public string? Remark { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        [Required]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string CreateUser { get; set; } = string.Empty;

        public DateTime? ModityTime { get; set; }
        [StringLength(50)]
        public string? ModifyUser { get; set; }

        public DateTime? DeleteTime { get; set; }
        [StringLength(50)]
        public string? DeleteUser { get; set; }

        // 導覽屬性
        public ICollection<Pond> Ponds { get; set; } = new List<Pond>();
    }
}
