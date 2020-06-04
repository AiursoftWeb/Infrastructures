using Newtonsoft.Json;

namespace Aiursoft.Identity.Services.Authentication.ToFaceBookServer
{
    public class FaceBookUserDetail : IUserDetail
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        public string AvatarUrl { get => Picture?.Data?.Url; set => Picture.Data.Url = value; }

        [JsonProperty("picture")]
        public FaceBookPicture Picture { get; set; } = new FaceBookPicture();

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("bio")]
        public string Bio { get; set; } = "";
    }

    public class FaceBookPicture
    {
        [JsonProperty("data")]
        public FaceBookPictureData Data { get; set; } = new FaceBookPictureData();
    }

    public class FaceBookPictureData
    {
        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
