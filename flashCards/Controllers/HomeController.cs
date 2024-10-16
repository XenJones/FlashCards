using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using flashCards.Models;

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
        return View();
    }

    public IActionResult AddFlashCard()
    {
        ViewData["Title"] = "Add Flashcard";
        ViewData["Action"] = "AddPack";
        ViewData["ButtonText"] = "Create Pack";

        return View("AddFlashCard", new FlashcardPack());
    }

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
                var saveResult = await _context.SaveChangesAsync();
                _logger.LogInformation($"SaveChanges result: {saveResult}");

                // Log the number of flashcards
                _logger.LogInformation($"Number of flashcards: {flashcardPack.Flashcards.Count}");

                // Redirect to the FlashCard view or another view after successful saving
                return RedirectToAction("FlashCard");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving FlashcardPack");
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact your system administrator.");
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
