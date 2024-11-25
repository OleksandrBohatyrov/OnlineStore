using Microsoft.AspNetCore.Mvc;
using OnlineStoreAPI.Data;
using OnlineStoreAPI.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineStoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly StoreContext _context;
        private readonly EmailService _emailService;

        public PaymentController(StoreContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

       
    }
}
