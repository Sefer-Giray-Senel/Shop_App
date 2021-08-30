using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NttProject.Data;
using NttProject.Dtos;
using NttProject.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NttProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet("logout")]
        public void Logout()
        {
            if (UserRepo.LoggedUser != null)
            {
                UserRepo.logout();
                Response.Redirect("/");
            }
            else
                Response.Redirect("/users/NoUser");
        }

        [HttpGet("NoUser")]
        public IActionResult NoUser()
        {
            return BadRequest("No user logged in.");
        }

        /*
        public PartialViewResult _partial()
        {
            User user = null;
            if (userRepo.LoggedUser != null)
            {
                Console.Write("asdfgh");
                user = new User();
                user.Username = userRepo.LoggedUser.Username;
            }
            
            return PartialView("_Layout", user);
        }
        */

        [HttpPost("register")]
        public ActionResult<UserDto> RegisterUser([FromForm]User user)
        {
            SqlConnection myConnection = new SqlConnection(@"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = product_app; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False");

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            
            sqlCmd.CommandText = $"INSERT INTO [dbo].[users](Name, Password) VALUES ( '{user.Username}' , '{user.Password}' );";

            sqlCmd.Connection = myConnection;
            myConnection.Open();
            SqlDataReader reader = sqlCmd.ExecuteReader();
            myConnection.Close();

            Response.Redirect("/");

            UserDto userDto = new UserDto
            {
                Username = user.Username,
                Token = getToken(user),
            };

            UserRepo.login(userDto);
            return userDto;
        }

        [HttpPost("login")]
        public ActionResult<UserDto> LoginUser([FromForm] User user)
        {
            SqlConnection myConnection = new SqlConnection(@"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = product_app; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False");

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;

            sqlCmd.CommandText = $"Select * from[dbo].[users] Where Name='{user.Username}' AND Password='{user.Password}';";

            sqlCmd.Connection = myConnection;
            myConnection.Open();
            SqlDataReader reader = sqlCmd.ExecuteReader();

            UserDto userDto = null;

            if (reader.Read())
            {
                userDto = new UserDto
                {
                    Username = user.Username,
                    Token = getToken(user),
                };

                UserRepo.login(userDto);

            }

            myConnection.Close();

            Response.Redirect("/");

            if (userDto == null)
                return BadRequest("Wrong username or password");
            else
                return userDto;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private string getToken(User user)
        {
            SymmetricSecurityKey key;
            key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TokenKeyTokenKeyTokenKeyTokenKey"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Username)
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}
