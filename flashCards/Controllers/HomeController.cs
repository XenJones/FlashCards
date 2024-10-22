using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using flashCards.Models;
using flashCards.Services;
using flashCards.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace flashCards.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<HomeController> _logger;
    private readonly UserService _userService;

    public HomeController(AppDbContext context, ILogger<HomeController> logger, UserService userService)
    {
        _context = context;
        _logger = logger;
        _userService = userService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult FlashCard()
    {
        var packs = _context.FlashcardPacks.ToList();
        return View(packs);
    }
    
    [Authorize]
    public IActionResult AddFlashCard()
    {
        ViewData["Title"] = "Add Flashcard";
        ViewData["Action"] = "AddPack";
        ViewData["ButtonText"] = "Create Pack";
        
        string userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        Console.WriteLine(userEmail);

        if (userEmail == null)
        {
            return RedirectToAction("Login", "Account");
        }
        
        var user = _userService.GetUserByEmail(userEmail);
        
        TempData["User"] = user.Id;

        return View("AddFlashCard", new FlashcardPack());
    }

    [Authorize]
    public IActionResult EditFlashCard()
    {
        ViewData["Title"] = "Edit Flash Card";
        ViewData["Action"] = "EditPack";
        ViewData["ButtonText"] = "Save Changes";
        
        string userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
        if (userEmail == null)
        {
            return RedirectToAction("Login", "Account");
        }
        var user = _userService.GetUserByEmail(userEmail);
        
        TempData["EditedUser"] = user.Id;

        return View("AddFlashCard", new FlashcardPack());
    }

    [HttpPost]
    [Authorize]
    public IActionResult ViewPack(Guid id)
    {
        var pack = _context.FlashcardPacks.Include(p => p.Flashcards).FirstOrDefault(
            p => p.Id == id);
        
        if (pack == null)
        {
            return NotFound();
        }

        return View("CardPack", pack);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public IActionResult deletePack(int id)
    {
        
        
        var pack = _context.FlashcardPacks.Find(id);

        _context.FlashcardPacks.Remove(pack);
        _context.SaveChanges();

        return RedirectToAction("FlashCard");
    }

    [HttpPost]
    public IActionResult DeleteAllPacks()
    {
        string userId = User.FindFirst(ClaimTypes.Name)?.Value;
        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }
        var user = _userService.GetUserById(userId);
        
        var allPacks = _context.FlashcardPacks.ToList();

        var userPacks = allPacks.FindAll(p => p.Id.ToString() == userId);
        
        _context.FlashcardPacks.RemoveRange(userPacks);

        _context.SaveChanges();

        return RedirectToAction("FlashCard");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPack(FlashcardPack flashcardPack)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _logger.LogInformation($"Attempting to add FlashcardPack: {flashcardPack.PackName}");

                // Ensure the Flashcards collection is initialized
                if (flashcardPack.Flashcards == null)
                {
                    flashcardPack.Flashcards = new List<Flashcard>();
                }

                // Remove any empty flashcards
                flashcardPack.Flashcards = flashcardPack.Flashcards
                    .Where(f => !string.IsNullOrWhiteSpace(f.Question) || !string.IsNullOrWhiteSpace(f.Answer))
                    .ToList();

                // Add the FlashcardPack to the context
                _context.FlashcardPacks.Add(flashcardPack);

                _logger.LogInformation($"Saving FlashcardPack to database...");
                // Save changes to get the FlashcardPack Id

                if (_context.FlashcardPacks.Where(x => x.PackName == flashcardPack.PackName).Any())
                {
                    TempData["Error"] = "Flashcard pack already exists";
                }
                else
                {

                    var saveResult = await _context.SaveChangesAsync();
                    _logger.LogInformation($"SaveChanges result: {saveResult}");

                    // Log the number of flashcards
                    _logger.LogInformation($"Number of flashcards: {flashcardPack.Flashcards.Count}");

                    // Redirect to the FlashCard view or another view after successful saving
                    return RedirectToAction("FlashCard");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving FlashcardPack");
                ModelState.AddModelError("",
                    "Unable to save changes. Try again, and if the problem persists, contact your system administrator.");
            }
        }

        if (!ModelState.IsValid)
        {
            foreach (var modelState in ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    _logger.LogError($"Validation error: {error.ErrorMessage}");
                }
            }
        }

        // Return the AddFlashCard view if there are validation issues or any errors
        ViewData["Title"] = "Add Flashcard";
        ViewData["Action"] = "AddPack";
        ViewData["ButtonText"] = "Create Pack";
        return View("AddFlashCard", flashcardPack);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
}
