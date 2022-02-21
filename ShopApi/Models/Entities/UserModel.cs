using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApi.Models.Entities
{
    public class UserModel : EntityModel
    {
        [Column("user_name")]
        public string Name { get; set; }

        [Column("password")]
        public string Password { get; set; }

        [Column("is_banned")]
        public bool IsBanned { get; set; }
    }
}
