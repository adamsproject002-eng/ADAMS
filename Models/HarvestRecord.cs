using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    /// <summary>
    /// 收穫紀錄資料表 HarvestRecord
    /// </summary>
    [Table("HarvestRecord")]
    public class HarvestRecord
    {
        [Key]
        public int HarvestRecordSN { get; set; }

        /// <summary>養殖池序號，對應 Pond 資料表</summary>
        public int PondSN { get; set; }

        /// <summary>放養碼</summary>
        [MaxLength(20)]
        public string FarmingCode { get; set; } = "";

        /// <summary>收穫日期</summary>
        [Column(TypeName = "date")]
        public DateTime HarvestDate { get; set; }

        /// <summary>收穫次別（P1~P5、Final）</summary>
        [MaxLength(10)]
        public string HarvestType { get; set; } = "";

        /// <summary>收穫重量(kg)</summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal HarvestWeight { get; set; }

        /// <summary>收穫尾數</summary>
        public int HarvestPCS { get; set; }

        /// <summary>ABW(g/pc)</summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal ABW { get; set; }

        /// <summary>DOC</summary>
        public int DOC { get; set; }

        /// <summary>管理員帳號</summary>
        [MaxLength(50)]
        public string? ManageAccount { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreateTime { get; set; }
        [MaxLength(50)]
        public string CreateUser { get; set; } = "";

        public DateTime? ModifyTime { get; set; }
        [MaxLength(50)]
        public string? ModifyUser { get; set; }

        public DateTime? DeleteTime { get; set; }
        [MaxLength(50)]
        public string? DeleteUser { get; set; }

        // 導覽屬性：一個 Pond 有很多 HarvestRecord
        [ForeignKey(nameof(PondSN))]
        public Pond? Pond { get; set; }
    }
}
