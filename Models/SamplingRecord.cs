using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ADAMS.Models
{
    /// <summary>
    /// 採樣資料表 SamplingRecord
    /// </summary>
    [Table("SamplingRecord")]
    public class SamplingRecord
    {
        [Key]
        public int SamplingRecordSN { get; set; }

        /// <summary>養殖池序號，對應 Pond 資料表的 PondSN</summary>
        public int PondSN { get; set; }

        /// <summary>放養碼</summary>
        [MaxLength(20)]
        public string? FarmingCode { get; set; }

        /// <summary>採樣日期</summary>
        [Column(TypeName = "date")]
        public DateTime SamplingDate { get; set; }

        /// <summary>採樣類別：Sample、P1~P5、Final</summary>
        [MaxLength(10)]
        public string SamplingType { get; set; } = string.Empty;

        /// <summary>採樣重量 (gms)</summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal SamplingWeight { get; set; }

        /// <summary>尾數 (pcs)</summary>
        public int SamplingPCS { get; set; }

        /// <summary>ABW</summary>
        [Column(TypeName = "decimal(10,2)")]
        public decimal ABW { get; set; }

        /// <summary>DOC：放苗日起算天數</summary>
        public int DOC { get; set; }

        /// <summary>管理員 Account</summary>
        [MaxLength(50)]
        public string? ManageAccount { get; set; }

        /// <summary>軟刪除旗標：0=否, 1=是</summary>
        public bool IsDeleted { get; set; }

        public string? Remark { get; set; }
        public DateTime CreateTime { get; set; }
        [MaxLength(50)]
        public string? CreateUser { get; set; }

        public DateTime? ModifyTime { get; set; }
        [MaxLength(50)]
        public string? ModifyUser { get; set; }

        public DateTime? DeleteTime { get; set; }
        [MaxLength(50)]
        public string? DeleteUser { get; set; }

        // 導覽屬性
        [ForeignKey(nameof(PondSN))]
        public Pond? Pond { get; set; }
    }
}
