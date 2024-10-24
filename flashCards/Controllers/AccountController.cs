using System.Security.Claims;
using flashCards.Models;
using flashCards.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace flashCards.Controllers;

public class AccountController : Controller

{
    private readonly UserService _userService;

    public AccountController(UserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(User model)
    {
        try
        {
            var user = await _userService.RegisterUser(model.UserName, model.Email, model.PasswordHash, model.FirstName, model.LastName,
                model.Phone);
            return RedirectToAction("Login");
        }
        catch (Exception e)
        {
            ModelState.AddModelError(string.Empty, e.Message);
            return View();
        }
        
    }
    
    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(User model)
    {
        var user = await _userService.AuthenticateUser(model.Email, model.PasswordHash);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
        };
        
        var identity = new ClaimsIdentity(claims, "CookieAuth");
        
        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
        
        await HttpContext.SignInAsync("CookieAuth", claimsPrincipal);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        return RedirectToAction("Login");  
    }

    [Authorize]
    public IActionResult Index()
    {
            string email = User.FindFirst(ClaimTypes.Email)?.Value;
            var user = _userService.GetUserByEmail(email);
            return View(user);
    }

    [HttpPost]
    public IActionResult Edit(User model)
    {
        
        _userService.UpdateUser(model);
        
        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public IActionResult Delete()
    {
        string userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

        if (userEmail == null)
        {
            return RedirectToAction("Login");
        }
        
        _userService.DeleteUser(userEmail);
        
        return RedirectToAction("Logout");
    }
    
}