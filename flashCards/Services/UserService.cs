using System.Security.Cryptography;
using flashCards.Migrations;
using flashCards.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace flashCards.Services;

public class UserService
{

    private readonly AppDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;
    
    public UserService(AppDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<User> RegisterUser(string username, string email, string password, string firstName, string lastName,
        string phoneNumber)
    {
        if (await _context.User.AnyAsync(u => u.Email == email))
        {
            throw new Exception($"Email {email} is already taken");
        }

        var user = new User
        {
            UserName = username,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Phone = phoneNumber,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
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
        
        return result == PasswordVerificationResult.Success ? user :
                throw new Exception($"Password is invalid");
    }

    public User GetUserByEmail(string email)
    {
        return _context.User.SingleOrDefault(u => u.Email == email);
    }

    public void UpdateUser(User user)
    {
        var existingUser = _context.User.SingleOrDefault(u => u.Email == user.Email);

        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.Phone = user.Phone;
        existingUser.UpdatedAt = DateTime.Now;
        existingUser.Email = user.Email;
        
        _context.Update(existingUser);
        _context.SaveChanges();
    }

    public void DeleteUser(string email)
    {
        var user = _context.User.SingleOrDefault(u => u.Email == email);
        _context.User.Remove(user);
        _context.SaveChanges();
    }
}
