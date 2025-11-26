using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    [Table("Area")]
    public class Area
    {
        [Key]
        public int AreaSN { get; set; }              // 序號，自動增加

        [Required]
        [StringLength(50)]
        public string AreaName { get; set; } = "";   // 場區編號 / 名稱

        [Required]
        public int TenantSN { get; set; }            // 所屬養殖戶 SN

        [ForeignKey(nameof(TenantSN))]
        public Tenant? Tenant { get; set; }

        [StringLength(20)]
        public string? GPSX { get; set; }            // 養殖場區代表 GPS 座標(經度 X)

        [StringLength(20)]
        public string? GPSY { get; set; }            // 養殖場區代表 GPS 座標(緯度 Y)

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

        // 導覽屬性：此場區底下的養殖池
        public ICollection<Pond> Ponds { get; set; } = new List<Pond>();
    }
}
