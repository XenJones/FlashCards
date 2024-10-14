using System.Security.Cryptography;
using flashCards.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace flashCards.Controllers;

public class UserService : Controller
{

    private readonly AppDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;
    
    public UserService(AppDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
    }

    public IActionResult Create()
    {
        return View();
    }

    public async Task<User> Register(string email, string password, string firstName, string lastName,
        string phoneNumber)
    {
        if (await _context.User.AnyAsync(u => u.Email == email))
        {
            throw new Exception($"Email {email} is already taken");
        }

        var user = new User
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Phone = phoneNumber,
        };
        
        user.PasswordHash = _passwordHasher.HashPassword(user, password);
        _context.Add(user);
        await _context.SaveChangesAsync();
        
        return user;
        

    }

    public async Task<User> AuthenticateUser(string email, string password)
    {
        var user = await _context.User.SingleOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            throw new Exception($"Email {email} is invalid");
        }
        
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        
        return result == PasswordVerificationResult.Success;
    }
    
}
