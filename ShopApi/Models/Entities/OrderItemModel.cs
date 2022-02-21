using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApi.Models.Entities;

public class OrderItemModel : OrderRecordBase
{
    [Column("order_id")]
    public long OrderId { get; set; }
}