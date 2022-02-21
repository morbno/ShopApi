using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApi.Models.Entities;

public abstract class OrderRecordBase : EntityModel
{
    [Column("shop_item_id")]
    public long ShopItemId { get; set; }
    
    [Column("quantity")]
    public int Quantity { get; set; }
}