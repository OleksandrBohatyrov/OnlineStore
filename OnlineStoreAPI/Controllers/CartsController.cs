using Microsoft.AspNetCore.Mvc;
using OnlineStoreAPI.Data;
using OnlineStoreAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace OnlineStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly StoreContext _context;

        public CartsController(StoreContext context)
        {
            _context = context;
        }

        [HttpPost("{userId}/add")]
        public async Task<ActionResult> AddToCart(int userId, int productId, int quantity)
        {
            var cart = await _context.Carts.Include(c => c.Items)
                                           .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem == null)
            {
                cartItem = new CartItem { ProductId = productId, Quantity = quantity, CartId = cart.Id };
                cart.Items.Add(cartItem);
                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
                _context.CartItems.Update(cartItem);
            }

            await _context.SaveChangesAsync();
            return Ok(cart);
        }


        [HttpGet("{userId}")]
        public async Task<ActionResult<Cart>> GetCart(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return NotFound("Cart not found.");
            }

            return Ok(cart);
        }
        [HttpDelete("{userId}/remove")]
        public async Task<ActionResult> RemoveFromCart(int userId, int productId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return NotFound("Cart not found.");
            }

            var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (cartItem == null)
            {
                return NotFound("Product not found in cart.");
            }

            cart.Items.Remove(cartItem);
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok(cart);
        }


    }
}
