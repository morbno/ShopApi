using System.ComponentModel.DataAnnotations;

namespace ShopApi.Models
{
    public class AuthenticateModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
