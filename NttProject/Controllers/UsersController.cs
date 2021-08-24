using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
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
        private readonly ILogger<HomeController> _logger;

        public UsersController(ILogger<HomeController> logger)
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
            if (reader.Read())
            {
                UserDto userDto = new UserDto
                {
                    Username = user.Username,
                    Token = getToken(user),
                };
                Response.Redirect("/");
                return userDto;
            }

            myConnection.Close();

            return BadRequest("Wrong username or password");            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private List<Product> getFromDb(string type)
        {
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection(@"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = product_app; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False");

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            switch (type)
            {
                case "electronics":
                    sqlCmd.CommandText = "Select * from [dbo].[products] Where category='electronics';";
                    break;
                case "clothing":
                    sqlCmd.CommandText = "Select * from [dbo].[products] Where category='clothing';";
                    break;
                case "furniture":
                    sqlCmd.CommandText = "Select * from [dbo].[products] Where category='furniture';";
                    break;
                default:
                    sqlCmd.CommandText = "Select * from [dbo].[products];";
                    break;
            }
            sqlCmd.Connection = myConnection;
            myConnection.Open();
            reader = sqlCmd.ExecuteReader();
            List<Product> list = new List<Product>();
            Product prod = null;
            while (reader.Read())
            {
                prod = new Product();
                prod.Id = Convert.ToInt32(reader.GetValue(0));
                prod.Name = reader.GetValue(1).ToString();
                prod.Category = reader.GetValue(2).ToString();
                list.Add(prod);
            }
            myConnection.Close();
            return list;
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
