using Microsoft.AspNetCore.Mvc;
using MotoStore.Api.Models;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserRepository _userRepo;
    private readonly AuthContext _auth;

    public AuthController(UserRepository userRepo, AuthContext auth)
    {
        _userRepo = userRepo;
        _auth = auth;
    }

    // Регистрация
    [HttpPost("register")]
    public IActionResult Register([FromBody] User user)
    {
        if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.PasswordHash))
            return BadRequest(new { message = "Username and password are required." });

        if (_userRepo.GetUserByUsername(user.Username) != null)
            return Conflict(new { message = "Username already exists." });

        user.PasswordHash = HashPassword(user.PasswordHash); // хешируем пароль
        user.Role = "customer"; // по умолчанию

        _userRepo.AddUser(user);
        return Ok(new { message = "User registered successfully." });
    }

    // Вход
    [HttpPost("login")]
    public IActionResult Login([FromBody] User user)
    {
        var existing = _userRepo.GetUserByUsername(user.Username);
        if (existing == null || HashPassword(user.PasswordHash) != existing.PasswordHash)
            return Unauthorized(new { message = "Invalid username or password." });

        _auth.SignIn(existing);

        return Ok(new
        {
            message = "Login successful.",
            username = existing.Username,
            role = existing.Role
        });
    }

    private string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
}
