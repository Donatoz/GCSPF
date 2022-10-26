namespace Neat.Extensions
{
    public static class NumericExtensions
    {
        public static float ToMegaBytes(this long size)
        {
            return size * 0.000001f;
        }
    }
}
