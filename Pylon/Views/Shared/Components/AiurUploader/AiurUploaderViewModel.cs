namespace Aiursoft.Pylon.Views.Shared.Components.AiurUploader
{
    public class AiurUploaderViewModel
    {
        public string Name { get; set; }
        public string PBToken { get; set; }
        public string SiteName { get; set; }
        public string Path { get; set; }
        public int SizeInMB { get; set; }
        public string AllowedExtensions { get; set; }
        public string DefaultFile { get; set; }
    }
}
