
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ECommerce.API.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        [Required]
        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [ForeignKey(nameof(OrderId))]
        [JsonIgnore]
        public Order? Order { get; set; }
    }
}