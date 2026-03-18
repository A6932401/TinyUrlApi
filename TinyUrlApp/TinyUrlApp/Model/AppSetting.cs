namespace TinyUrlApp.Model
{
    public class AppSetting
    {
        public const string SECNAME = "App";
        public string blobStorageConnection { get; set; }    
        public string blobStorageContainer { get; set; }    
        public string blobStorageFileFormat { get; set; }    
        public string CorsOrigins { get; set; }    
        public string dbConnection { get; set; }    
        public string baseUrl { get; set; }
    }
}
