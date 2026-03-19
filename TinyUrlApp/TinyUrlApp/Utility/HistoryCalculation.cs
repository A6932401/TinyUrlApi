namespace TinyUrlApp.Utility
{
    public static class HistoryCalculation
    {
        public static string HistoryCal(this DateTime CreatedDate)
        {
            var diff = DateTime.Now - CreatedDate;

            if (diff.TotalMinutes < 1)
                return "just now";
            if (diff.TotalHours < 1)
                return $"{(int)diff.TotalMinutes} min ago";
            if (diff.TotalDays < 1)
                return $"{(int)diff.TotalHours} hr ago";
            if (diff.TotalDays < 30)
                return $"{(int)diff.TotalDays} day ago";

            return CreatedDate.ToString("yyyy-MM-dd");
        }
    }
}
