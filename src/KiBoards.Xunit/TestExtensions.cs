using System.Security.Cryptography;
using System.Text;

namespace KiBoards
{
    internal static class TestExtensions
    {     
        public static string ComputeMD5(this string value) => BitConverter.ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(value))).Replace("-", "").ToLower();
    }
}
