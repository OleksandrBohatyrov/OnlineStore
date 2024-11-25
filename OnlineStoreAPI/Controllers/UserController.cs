using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineStoreAPI.Data;
using OnlineStoreAPI.Models;
using SolomikovPod.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OnlineStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly StoreContext _context;

        public UsersController(StoreContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(LoginRequest userRequest)
        {
            if (_context.Users.Any(u => u.Username == userRequest.Username))
            {
                return BadRequest("Username already exists.");
            }

            if (string.IsNullOrEmpty(userRequest.Password))
            {
                return BadRequest("Password is required.");
            }

            // Hash the password and create the user object
            var user = new User
            {
                Username = userRequest.Username,
                PasswordHash = HashPassword(userRequest.Password), // Only store the hash
                Email = userRequest.Email // Assuming userRequest has an Email field
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }




        [HttpPost("login")]
        public async Task<ActionResult<User>> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == loginRequest.Username);

            if (user == null)
            {
                return BadRequest("Invalid username or password.");
            }

            if (!VerifyPassword(loginRequest.Password, user.PasswordHash))
            {
                return BadRequest("Invalid username or password.");
            }

            return Ok(user);
        }


        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }

        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }

            public string Email { get; set; }
        }
    }
}
