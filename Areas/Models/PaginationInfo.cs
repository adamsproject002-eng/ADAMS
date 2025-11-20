namespace ADAMS.Areas.Models
{
    public class PaginationInfo
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public int[] PageSizeOptions { get; set; } = new[] { 5, 10, 15, 20 };

        /// <summary>
        /// 存放所有搜索條件
        /// Key: 參數名稱 (如 "statusFilter", "keyword", "startDate")
        /// Value: 參數值 (支援任何類型)
        /// </summary>
        public Dictionary<string, object> SearchFilters { get; set; } = new();
    }
}
