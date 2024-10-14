using Microsoft.AspNetCore.Mvc;

namespace flashCards.Controllers;

public class UserController : Controller
{
    public IActionResult Create()
    {
        return View();
    }

    public IActionResult CreatePost()
    {
        return View();
    }
}