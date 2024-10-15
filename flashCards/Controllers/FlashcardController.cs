using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlashcardApp.Controllers
{
    public class FlashcardController : Controller
    {
        private readonly AppDbContext _context;

        public FlashcardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}