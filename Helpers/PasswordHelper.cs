namespace ADAMS.Helpers
{
    public class PasswordHelper
    {
        public static string Sha256(string s)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            return Convert.ToHexString(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(s)));
        }
    }
}
