using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string DeliveryAddress { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Pending";

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}