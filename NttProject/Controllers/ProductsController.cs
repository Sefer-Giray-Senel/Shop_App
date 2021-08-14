using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NttProject.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NttProject.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class ProductsController : Controller
    {
        private readonly IConfiguration _config;

        public ProductsController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        [Route("get")]
        public ActionResult<IEnumerable<Product>> Get1()
        {
            //return listEmp.First(e => e.ID == id);  
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection(@"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = product_app; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False");

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = "Select * from [dbo].[products];";
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
                list.Add(prod);
            }
            myConnection.Close();
            return list;
        }

        /*
        [HttpGet]
        [Route("getid")]
        public Product Get2(int id)
        {
            //return listEmp.First(e => e.ID == id);  
            SqlDataReader reader = null;
            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = @"Data Source = (localdb)\MSSQLLocalDB; Initial Catalog = master; Integrated Security = True; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False";

            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = "Select * from product_table where ProductId=" + id + "";
            sqlCmd.Connection = myConnection;
            myConnection.Open();
            reader = sqlCmd.ExecuteReader();
            Product prod = null;
            while (reader.Read())
            {
                prod = new Product();
                prod.ProductId = Convert.ToInt32(reader.GetValue(0));
                prod.Name = reader.GetValue(1).ToString();
            }
            myConnection.Close();
            return prod;
        }

        
        [HttpPost]
        [ActionName("AddEmployee")]
        public void AddEmployee(Product prod)
        {
            //int maxId = listEmp.Max(e => e.ID);  
            //employee.ID = maxId + 1;  
            //listEmp.Add(employee);  


            SqlConnection myConnection = new SqlConnection();
            myConnection.ConnectionString = @"Server=.\SQLSERVER2008R2;Database=DBCompany;User ID=sa;Password=xyz@1234;";
            //SqlCommand sqlCmd = new SqlCommand("INSERT INTO tblEmployee (EmployeeId,Name,ManagerId) Values (@EmployeeId,@Name,@ManagerId)", myConnection);  
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = "INSERT INTO product_table (EmployeeId,Name) Values (@ProductId,@Name)";
            sqlCmd.Connection = myConnection;


            sqlCmd.Parameters.AddWithValue("@EmployeeId", prod.ProductId);
            sqlCmd.Parameters.AddWithValue("@Name", prod.Name);
            myConnection.Open();
            int rowInserted = sqlCmd.ExecuteNonQuery();
            myConnection.Close();
        }
        */
        
    }
}
