using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using flashCards.Models;
using Microsoft.EntityFrameworkCore;

namespace flashCards.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(AppDbContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
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

    public IActionResult Slideshow(int id)
    {
        var pack = _context.FlashcardPacks
            .Include(p => p.Flashcards)
            .FirstOrDefault(p => p.Id == id);
    
        if (pack == null)
        {
            return NotFound();
        }
    
        return View(pack);
    }

    public IActionResult AddFlashCard()
    {
        ViewData["Title"] = "Add Flashcard";
        ViewData["Action"] = "AddPack";
        ViewData["ButtonText"] = "Create Pack";

        return View("AddFlashCard", new FlashcardPack());
    }

    public IActionResult EditFlashCard(int id)
    {
        var pack = _context.FlashcardPacks
            .Include(p => p.Flashcards)
            .FirstOrDefault(p => p.Id == id);

        if (pack == null)
        {
            return NotFound();
        }

        ViewData["Title"] = "Edit Flash Card";
        ViewData["Action"] = "EditPack";
        ViewData["ButtonText"] = "Save Changes";

        return View("AddFlashCard", pack);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditPack(FlashcardPack flashcardPack)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var existingPack = await _context.FlashcardPacks
                    .Include(p => p.Flashcards)
                    .FirstOrDefaultAsync(p => p.Id == flashcardPack.Id);

                if (existingPack == null)
                {
                    return NotFound();
                }

                // Update pack properties
                existingPack.PackName = flashcardPack.PackName;

                // Remove existing flashcards
                _context.Flashcards.RemoveRange(existingPack.Flashcards);

                // Add new flashcards
                existingPack.Flashcards = flashcardPack.Flashcards
                    .Where(f => !string.IsNullOrWhiteSpace(f.Question) || !string.IsNullOrWhiteSpace(f.Answer))
                    .ToList();

                await _context.SaveChangesAsync();
                return RedirectToAction("FlashCard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating FlashcardPack");
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact your system administrator.");
            }
        }

        ViewData["Title"] = "Edit Flash Card";
        ViewData["Action"] = "EditPack";
        ViewData["ButtonText"] = "Save Changes";
        return View("AddFlashCard", flashcardPack);
    }


    [HttpPost]
    public IActionResult ViewPack(int id)
    {
        var pack = _context.FlashcardPacks.Include(p => p.Flashcards).FirstOrDefault(p => p.Id == id);

        if (pack == null)
        {
            return NotFound();
        }

        return View("CardPack", pack);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
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
        var allPacks = _context.FlashcardPacks.ToList();

        _context.FlashcardPacks.RemoveRange(allPacks);

        _context.SaveChanges();

        return RedirectToAction("FlashCard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddPack(FlashcardPack flashcardPack)
    {
        if (ModelState.IsValid)
        {
            try
            {
                _logger.LogInformation($"Attempting to save FlashcardPack: {flashcardPack.PackName}");

                // Ensure the Flashcards collection is initialized
                if (flashcardPack.Flashcards == null)
                {
                    flashcardPack.Flashcards = new List<Flashcard>();
                }

                // Remove any empty flashcards
                flashcardPack.Flashcards = flashcardPack.Flashcards
                    .Where(f => !string.IsNullOrWhiteSpace(f.Question) || !string.IsNullOrWhiteSpace(f.Answer))
                    .ToList();

                if (flashcardPack.Id == 0) // New pack
                {
                    _context.FlashcardPacks.Add(flashcardPack);
                }
                else // Update existing pack
                {
                    _context.FlashcardPacks.Update(flashcardPack);
                }

                _logger.LogInformation($"Saving FlashcardPack to database...");
                var saveResult = await _context.SaveChangesAsync();
                _logger.LogInformation($"SaveChanges result: {saveResult}");

                return RedirectToAction("FlashCard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving FlashcardPack");
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact your system administrator.");
            }
        }

        // Log validation errors if any
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
        ViewData["Title"] = flashcardPack.Id == 0 ? "Add Flashcard" : "Edit Flashcard";
        ViewData["Action"] = flashcardPack.Id == 0 ? "AddPack" : "EditPack";
        ViewData["ButtonText"] = flashcardPack.Id == 0 ? "Create Pack" : "Save Changes";

        return View("AddFlashCard", flashcardPack);
    }


    [HttpGet]
    public IActionResult ViewSearchPack(int id)
    {
        var pack = _context.FlashcardPacks.Include(p => p.Flashcards).FirstOrDefault(p => p.Id == id);

        if (pack == null)
        {
            return NotFound();
        }

        return View("CardPack", pack);
    }

    [HttpGet]
    public IActionResult SearchPacks(string query)
    {
        if (string.IsNullOrEmpty(query))
        {
            return Json(new List<object>());
        }

        var packs = _context.FlashcardPacks
            .Where(p => p.PackName.Contains(query))
            .Select(p => new { id = p.Id, name = p.PackName })
            .ToList();

        return Json(packs);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
