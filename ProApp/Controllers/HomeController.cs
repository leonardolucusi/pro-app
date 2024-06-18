using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProApp.Data;
using ProApp.Models;
using ProApp.Models.Entities;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly Context _context;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, Context context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegisterUser(UserRegister userRegister)
        {
            if (ModelState.IsValid)
            {
                var userToFind = _context.Users.FirstOrDefault(u => u.Username == userRegister.Username);
                if (userToFind != null)
                {
                    if (userRegister.Username == userToFind.Username) { return BadRequest("Username already exists"); }
                }

                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegister.Password);
                var user = new User
                {
                    Username = userRegister.Username,
                    Password = hashedPassword
                };
                _context.Users.Add(user);
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost]
        public IActionResult LoginUser(UserLogin user)
        {
            if (ModelState.IsValid)
            {
                var userToBeFound = _context.Users.FirstOrDefault(u => u.Username == user.Username);

                if (userToBeFound != null && BCrypt.Net.BCrypt.Verify(user.Password, userToBeFound.Password))
                {
                    var token = GenerateJwtToken(user.Username);

                    HttpContext.Response.Cookies.Append("JwtToken", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true
                    });
                    return Redirect(Url.Action("ProtectedRoute", "Home"));
                }
                ModelState.AddModelError("", "Invalid Credentials");
            }
            return View("Login", user);
            //if (ModelState.IsValid)
            //{
            //    var userToBeFound = _context.Users.FirstOrDefault(u => u.Username == user.Username && u.Password == user.Password);

            //    if (userToBeFound != null)
            //    {
            //        var token = GenerateJwtToken(user.Username);

            //        HttpContext.Response.Cookies.Append("JwtToken", token, new CookieOptions
            //        {
            //            HttpOnly = true,
            //            Secure = true
            //        });
            //        return Redirect(Url.Action("ProtectedRoute", "Home"));
            //    }
            //    ModelState.AddModelError("", "Invalid Credentials");
            //}
            //return View("Login", user);
        }

        private string GenerateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, username),
                new Claim("username", username)
            }),
                Expires = DateTime.UtcNow.AddSeconds(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        [HttpGet]
        [Authorize]
        public IActionResult ProtectedRoute()
        {
            var token = Request.Cookies["JwtToken"];
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Token is missing");
            }

            var handler = new JwtSecurityTokenHandler();
            try
            {
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
                if (jsonToken == null) return Unauthorized("Invalid token.");

                foreach (var claim in jsonToken.Claims)
                {
                    Debug.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                }
                var usernameClaim = jsonToken.Claims.FirstOrDefault(claim => claim.Type == "username");
                if (usernameClaim == null)
                {
                    return Unauthorized("Username not found in token.");
                }

                ViewBag.Username = usernameClaim.Value;

                return View();
            }
            catch (Exception ex)
            {
                return Unauthorized($"Error processing token: {ex.Message}");
            }
        }
    }
}
