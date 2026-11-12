
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerce.API.Data;
using ECommerce.API.Models;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public OrdersController(ECommerceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = await _context.Orders
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            if (order.Items == null || order.Items.Count == 0)
            {
                return BadRequest("An order must contain at least one item.");
            }

            if (string.IsNullOrWhiteSpace(order.CustomerName) ||
                string.IsNullOrWhiteSpace(order.Email) ||
                string.IsNullOrWhiteSpace(order.Phone) ||
                string.IsNullOrWhiteSpace(order.DeliveryAddress))
            {
                return BadRequest("Please provide all customer details.");
            }

            foreach (var item in order.Items)
            {
                if (item.ProductId <= 0 || item.Quantity <= 0)
                {
                    return BadRequest("Invalid product or quantity.");
                }
            }

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var requestedItems = order.Items
                    .GroupBy(item => item.ProductId)
                    .Select(group => new
                    {
                        ProductId = group.Key,
                        Quantity = group.Sum(item => item.Quantity)
                    })
                    .ToList();

                var productIds = requestedItems
                    .Select(item => item.ProductId)
                    .ToList();

                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id);

                foreach (var requestedItem in requestedItems)
                {
                    if (!products.TryGetValue(
                        requestedItem.ProductId, out var product))
                    {
                        return BadRequest(
                            $"Product {requestedItem.ProductId} was not found.");
                    }

                    if (product.StockQuantity < requestedItem.Quantity)
                    {
                        return Conflict(
                            $"Not enough stock for {product.Name}. " +
                            $"Available stock: {product.StockQuantity}");
                    }
                }

                order.Id = 0;
                order.OrderDate = DateTime.UtcNow;
                order.Status = "Pending";

                order.Items = new List<OrderItem>();

                foreach (var requestedItem in requestedItems)
                {
                    var product = products[requestedItem.ProductId];

                    product.StockQuantity -= requestedItem.Quantity;

                    order.Items.Add(new OrderItem
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = requestedItem.Quantity,
                        UnitPrice = product.Price
                    });
                }

                order.TotalAmount = order.Items.Sum(
                    item => item.UnitPrice * item.Quantity);

                _context.Orders.Add(order);

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return CreatedAtAction(
                    nameof(GetOrder),
                    new { id = order.Id },
                    order);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
