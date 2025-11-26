using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    [Table("Pond")]
    public class Pond
    {
        [Key]
        public int PondSN { get; set; }                     // 序號，自動增加

        [Required]
        public int AreaSN { get; set; }                     // 場區序號 (FK to Area)

        [ForeignKey(nameof(AreaSN))]
        public Area? Area { get; set; }

        [Required]
        public int TenantSN { get; set; }                   // 所屬養殖戶 SN

        [ForeignKey(nameof(TenantSN))]
        public Tenant? Tenant { get; set; }

        [Required]
        [StringLength(20)]
        public string PondNum { get; set; } = "";           // 池號

        [Column(TypeName = "decimal(10,2)")]
        public decimal PondWidth { get; set; }              // 最寬邊(m)

        [Column(TypeName = "decimal(10,2)")]
        public decimal PondLength { get; set; }             // 長 / 直徑(m)

        [Column(TypeName = "decimal(10,2)")]
        public decimal PondArea { get; set; }               // 水面積(m2)

        [StringLength(20)]
        public string? GPSX { get; set; }                   // 養殖池 GPS 座標(經度 X)

        [StringLength(20)]
        public string? GPSY { get; set; }                   // 養殖池 GPS 座標(緯度 Y)

        [StringLength(100)]
        public string? Remark { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

        [Required]
        public DateTime CreateTime { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string CreateUser { get; set; } = "";

        public DateTime? ModifyTime { get; set; }

        [StringLength(50)]
        public string? ModifyUser { get; set; }

        public DateTime? DeleteTime { get; set; }

        [StringLength(50)]
        public string? DeleteUser { get; set; }
    }
}
