using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApi.Models.Entities;

public class ShopCartModel : OrderRecordBase
{
    [Column("user_id")]
    public long UserId { get; set; }
}