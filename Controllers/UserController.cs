using LibraryAPI.DTOs;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly LibraryContext _context;
        private readonly IConfiguration _config;

        public UserController(LibraryContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRegisterDto userDto)
        {
            if (_context.Users.Any(u => u.Username == userDto.Username))
            {
                return BadRequest(new
                {
                    status = 400,
                    message = "Username already taken"
                });
            }

            CreatePasswordHash(userDto.Password, out byte[] hash, out byte[] salt);

            var user = new User
            {
                Username = userDto.Username,
                PasswordHash = hash,
                PasswordSalt = salt
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok($"User {user.Username} successfully registered");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserRegisterDto userDto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == userDto.Username);

            if (user == null)
            {
                return Unauthorized(new
                {
                    status = 401,
                    message = "Invalid username or password"
                });
            }

            if (!VerifyPasswordHash(userDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                return Unauthorized(new
                {
                    status = 401,
                    message = "Invalid username or password"
                });
            }

            var token = CreateToken(user);

            return Ok(token);
        }

        private void CreatePasswordHash(string password, out byte[] hash, out byte[] salt)
        {
            using var hmac = new HMACSHA512();
            salt = hmac.Key;
            hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computed.SequenceEqual(storedHash);
        }

        private string CreateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
