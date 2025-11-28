using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    /// <summary>
    /// 投餌規劃主表
    /// </summary>
    [Table("FeedingPlanMain")]
    public class FeedingPlanMain
    {
        [Key]
        public int FPMSN { get; set; }          // 序號，自動增加

        [Required]
        public int TenantSN { get; set; }       // 所屬養殖戶 SN

        [ForeignKey(nameof(TenantSN))]
        public Tenant? Tenant { get; set; }

        [Required]
        [StringLength(50)]
        public string FeedingPlanName { get; set; } = string.Empty; // 投餌規劃名稱

        [Required]
        public int FVSN { get; set; }           // 魚種序號(對應 FishVariety)

        [ForeignKey(nameof(FVSN))]
        public FishVariety? FishVariety { get; set; }

        [StringLength(100)]
        public string? Remark { get; set; }

        [Required]
        public bool IsDeleted { get; set; } = false;

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

        public ICollection<FeedingPlanDetail> Details { get; set; } = new List<FeedingPlanDetail>();
    }
}
