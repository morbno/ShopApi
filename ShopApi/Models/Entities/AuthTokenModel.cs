using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApi.Models.Entities;

public class AuthTokenModel : EntityModel
{
    [Column("user_id")]
    public long UserId { get; set; }
    
    [Column("token")]
    public string Token { get; set; }
    
    [Column("date_issued")]
    public DateTime DateIssued { get; set; }
    
    [Column("date_expire")]
    public DateTime DateExpire { get; set; }
}