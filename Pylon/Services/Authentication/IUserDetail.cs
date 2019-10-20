using System;
using System.Collections.Generic;
using System.Text;

namespace Aiursoft.Pylon.Services.Authentication
{
    public interface IUserDetail
    {
        public int Id { get; set; }
        public string AvatarUrl { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public string Email { get; set; }
    }
}
