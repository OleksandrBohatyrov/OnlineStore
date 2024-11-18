using Microsoft.AspNetCore.Mvc;
using OnlineStoreAPI.Data;
using OnlineStoreAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace OnlineStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly StoreContext _context;

        public PaymentController(StoreContext context)
        {
            _context = context;
        }

        [HttpPost("{userId}/checkout")]
        public async Task<ActionResult> Checkout(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || cart.Items.Count == 0)
            {
                return BadRequest("denis loh suka.");
            }

            foreach (var item in cart.Items)
            {
                if (item.Product.Stock < item.Quantity)
                {
                    return BadRequest($"Not enough stock for product {item.Product.Name}.");
                }

                item.Product.Stock -= item.Quantity;
            }

            cart.Items.Clear();
            await _context.SaveChangesAsync();

            return Ok(new { message = "daniil chmo !", currency = "MyCoin" });
        }
    }
}
