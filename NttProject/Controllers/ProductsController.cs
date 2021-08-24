using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NttProject.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NttProject.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public ProductsController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult All()
        {
            List<Product> list = getFromDb("home");
            ViewData["List"] = list;
            return View(list);
        }

        public IActionResult Electronics()
        {
            List<Product> list = getFromDb("electronics");
            ViewData["List"] = list;
            return View(list);
        }
        public IActionResult Clothing()
        {
            List<Product> list = getFromDb("clothing");
            ViewData["List"] = list;
            return View(list);
        }

        [Authorize]
        public IActionResult Furniture()
        {
            List<Product> list = getFromDb("furniture");
            ViewData["List"] = list;
            return View(list);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public List<Product> getFromDb(string type)
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

    }
}
