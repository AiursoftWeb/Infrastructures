using Newtonsoft.Json;
using System;

namespace Aiursoft.Pylon.Services.Authentication.ToGoogerServer
{
    public class GoogleUserDetail : IUserDetail
    {
        [JsonProperty("sub")]
        public string Sub { get; set; }

        [JsonProperty("picture")]
        public string AvatarUrl { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("bio")]
        public string Bio { get; set; }
        int IUserDetail.Id { get => Convert.ToInt32(Sub); set => Sub = value.ToString(); }
    }
}
