namespace ADAMS.ViewModels
{
    public class UserProfileViewModel
    {
        public string AccountName { get; set; } = string.Empty;
        public string? TenantName { get; set; }
        public string? GroupName { get; set; }
        public List<string> FunctionNames { get; set; } = new();
    }
}
