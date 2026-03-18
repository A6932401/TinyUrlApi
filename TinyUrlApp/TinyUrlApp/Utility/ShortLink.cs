namespace TinyUrlApp.Utility
{
    public static class ShortLink
    {
        private static readonly Random _random = Random.Shared; 
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        public static string GetShortLink(this string baseUrl)
        {
           var code = new string(Enumerable.Repeat(Chars, 6)
            .Select(s => s[_random.Next(s.Length)])
            .ToArray());

            return $"{baseUrl.TrimEnd('/')}/{code}";           
        }
    }
}
