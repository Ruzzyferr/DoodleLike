using DoodleLike.Models;
using Microsoft.AspNetCore.Mvc;

namespace DoodleLike.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        // UserController içinde gerekli işlevlerin implementasyonu...
    }
}
