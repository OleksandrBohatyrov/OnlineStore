﻿using Microsoft.AspNetCore.Mvc;
using OnlineStoreAPI.Data;
using OnlineStoreAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using OnlineStoreAPI.Services;

namespace OnlineStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly StoreContext _context;
        private readonly EmailService _emailService; // Добавляем EmailService

        public CartsController(StoreContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService; // Внедрение зависимости EmailService через конструктор
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<Cart>> GetCart(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product) // Загружаем данные о продукте
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return NotFound("Cart not found.");
            }

            return Ok(cart); // Возвращаем полную модель Cart
        }
        [HttpPost("{userId}/checkout")]
        public async Task<ActionResult> Checkout(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.Items.Any())
            {
                return NotFound("Cart not found or empty.");
            }

            // Проверка наличия товаров на складе
            foreach (var item in cart.Items)
            {
                if (item.Product.Stock < item.Quantity)
                {
                    return BadRequest($"Not enough stock for product {item.Product.Name}.");
                }
            }

            // Формирование списка купленных товаров для email и подсчёт итоговой суммы
            var purchasedItems = new List<(string ProductName, int Quantity, decimal Price)>();
            decimal totalAmount = 0;

            foreach (var item in cart.Items)
            {
                purchasedItems.Add((item.Product.Name, item.Quantity, item.Product.Price));
                totalAmount += item.Quantity * item.Product.Price;
            }

            // Обновляем количество товаров на складе
            foreach (var item in cart.Items)
            {
                item.Product.Stock -= item.Quantity;
            }

            // Очистка корзины
            _context.CartItems.RemoveRange(cart.Items);
            await _context.SaveChangesAsync();

            // Получение данных покупателя для отправки email
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            // Отправка письма о подтверждении оплаты с корректными данными
            _emailService.SendPaymentConfirmationEmail(user.Email, user.Username, purchasedItems, totalAmount);

            return Ok("Payment successful and confirmation email sent.");
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

        [HttpPost("{userId}/add")]
        public async Task<ActionResult> AddToCart(int userId, [FromBody] CartItemDto cartItemDto)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId, Items = new List<CartItem>() };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            var existingCartItem = cart.Items.FirstOrDefault(i => i.ProductId == cartItemDto.ProductId);
            if (existingCartItem != null)
            {
                existingCartItem.Quantity += cartItemDto.Quantity;
            }
            else
            {
                var newCartItem = new CartItem
                {
                    ProductId = cartItemDto.ProductId,
                    Quantity = cartItemDto.Quantity
                };
                cart.Items.Add(newCartItem);
                _context.CartItems.Add(newCartItem);
            }

            await _context.SaveChangesAsync();
            return Ok(cart);
        }

        public class CartItemDto
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }
    }
}
