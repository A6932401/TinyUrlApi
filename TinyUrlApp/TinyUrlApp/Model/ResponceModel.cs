using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TinyUrlApp.Model
{
    public class ResponceModel<T>
    {
        public string status { get; set; }
        public string message { get; set; }
        public T response { get; set; }
    }
}
