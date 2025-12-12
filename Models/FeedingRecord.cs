// Models/FeedingRecord.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    [Table("FeedingRecord")]
    public class FeedingRecord
    {
        [Key]
        public int FeedingRecordSN { get; set; }   // 序號，自動增加

        [Required]
        public int PondSN { get; set; }            // 養殖池序號 (FK → Pond)

        [ForeignKey(nameof(PondSN))]
        public Pond? Pond { get; set; }

        [Required]
        [StringLength(20)]
        public string FarmingCode { get; set; } = ""; // 放養碼

        [Required]
        public DateTime FeedingDate { get; set; }     // 投餌日期

        [Required]
        public int TimeZoneSN { get; set; }           // 投料時間的TimeZoneSN，對應時間區段資料表的TimeZoneSN
        [ForeignKey(nameof(TimeZoneSN))]
        public TimeZone? TimeZone { get; set; }
        [Required]
        public int FeedSN { get; set; }               //投餌飼料的FeedSN，對應飼料資料表(Feed)的FeedSN

        [ForeignKey(nameof(FeedSN))]
        public Feed? Feed { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal FeedingAmount { get; set; }    // 投料數量

        [Required]
        [StringLength(20)]
        public string Unit { get; set; } = "";        // 單位

        [Column(TypeName = "decimal(10,2)")]
        public decimal? SurvivalRate { get; set; }    // 估計存活率

        [Column(TypeName = "decimal(10,2)")]
        public decimal ABW { get; set; }              // ABW

        public int DOC { get; set; }                  // DOC

        [StringLength(50)]
        public string ManageAccount { get; set; } = ""; // 管理員 Account

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
