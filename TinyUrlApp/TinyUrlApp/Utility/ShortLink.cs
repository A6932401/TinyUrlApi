namespace TinyUrlApp.Utility
{
    public static class ShortLink
    {
        public static string GetShortLink(this string fullLink)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());            
        }
    }
}
