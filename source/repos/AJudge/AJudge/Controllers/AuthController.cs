using Microsoft.AspNetCore.Mvc;
using AJudge.Domain.Entities;
using AJudge.Infrastructure.Data;
using AJudge.Application.services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AJudge.Application.DTO.AuthDTO;

namespace AJudge.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthController> _logger;
        private readonly PasswordHasher _passwordHasher;
        public readonly IConfiguration _configuration;
        public AuthController(ApplicationDbContext context, ILogger<AuthController> logger, PasswordHasher passwordHasher )
        {
            _context = context;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
          
            if (_context.Users.Any(u => u.Email == request.Email|| u.Username==request.Username))
            {
                return BadRequest("User with this email already exists.");
            }

            // Hash the incoming password
            var hashedPassword = _passwordHasher.Hash(request.Password);

            // Create and save the new user
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = hashedPassword,
                RegisterAt = DateTime.UtcNow
                
            };

             await _context.Users.AddAsync(user);
            await  _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Retrieve the user by email
            var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }
            
            // Verify the password against the stored hash
            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                return Unauthorized("Invalid email or password.");
            }

            // Generate JWT token
            // var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var config= new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var jwtOption =config.GetSection("JWT").Get<JwtOptions>();   
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtOption.SigningKey); 
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                            new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddHours(jwtOption.LifeTime),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(tokenString);
        }
    }

   
}
