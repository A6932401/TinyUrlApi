using TinyUrlApp.Utility;

namespace TinyUrlApp.Model
{
    public class LinkAdd
    {
        public string originalUrl { get; set; }
        public bool isPrivate { get; set; }

    }

    public class ReturnLink
    {
        public int id { get; set; }
        public string shortlink { get; set; }
        public string originallink { get; set; }
        public int clickcount { get; set; }
        public string duration { get { return createddate.HistoryCal(); } } 
        public DateTime createddate { get; set; }
    }
}
