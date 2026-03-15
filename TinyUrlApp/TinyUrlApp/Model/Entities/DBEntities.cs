using Dapper.Contrib.Extensions;

namespace TinyUrlApp.Model.Entities
{
    [Table("EndPoint")]
    public class EndPoint
    {
        public string shortlink { get; set; }
        public string originallink { get; set; }
        public bool isprivate { get; set; }
        public string status { get; set; }
        public int clickcount { get; set; }
        public DateTime createddate { get; set; }
    }

}
