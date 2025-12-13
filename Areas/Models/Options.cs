namespace ADAMS.Areas.Models
{
    /// <summary>
    /// 養殖戶下拉選單共用選項
    /// </summary>
    public class TenantOption
    {
        public int SN { get; set; }
        public string? TenantName { get; set; }
    }
    /// <summary>
    /// 場區下拉選項
    /// </summary>
    public class AreaOption
    {
        public int AreaSN { get; set; }
        public string AreaName { get; set; } = "";
    }

    public class PondOption
    {
        public int PondSN { get; set; }
        public string PondNum { get; set; } = "";
    }

    // 下拉清單用的小 Option 類別
    public class TimeZoneOptionVM
    {
        public int TimeZoneSN { get; set; }
        public string DisplayText { get; set; } = "";
    }

    public class FeedOption
    {
        public int FeedSN { get; set; }
        public string FeedName { get; set; } = ""; // ex: "CP 7704"
        public string UnitName { get; set; } = "";
    }
    public class SupplierOption
    {
        public int SupplierSN { get; set; }
        public string SupplierName { get; set; } = "";
    }

    public class AccountOption
    {
        public int AccountSN { get; set; }
        public string AccountName { get; set; } = "";
    }

    /// <summary>
    /// 採樣 / 收穫次別下拉選項
    /// 例如：Sample、P1、P2…、Final
    /// </summary>
    public class StageOption
    {
        public string Value { get; set; } = "";
        public string Text { get; set; } = "";
    }
}
