namespace Common.Utils
{
    public class UUID
    {
        public static string CreateNumberSeries(int length = 16)
        {
            const string digits = "0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(digits, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string CreateAlphanumericSeries(int length = 16)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
