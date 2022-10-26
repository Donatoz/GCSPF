namespace Neat.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string str) => str == null || str.Trim().Length == 0;
    }
}
