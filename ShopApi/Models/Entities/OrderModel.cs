using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopApi.Models.Entities
{
    public class OrderModel : EntityModel
    {
        [Column("user_id")]
        public long UserId { get; set; }

        [Column("status", TypeName = "e_order_status")]
        public OrderStatus Status { get; set; }
        
        [Column("price")]
        public decimal Price { get; set; }
    }
}
