using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication6.Data;
using WebApplication6.Models;
using WebApplication6.Models.Entities;

namespace WebApplication6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        public EmployeesController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllEmployees()
        {
            var allEmployees = dbContext.Employees.ToList();
            return Ok(allEmployees);
        }

        [HttpPost]
        public IActionResult AddEmployee(AddEmployeeDto addEmployee)
        {
            var employee = new Employee()
            {
                Name = addEmployee.Name,
                Email = addEmployee.Email,
                Phone = addEmployee.Phone,
                Salary = addEmployee.Salary,
            };
            dbContext.Employees.Add(employee);
            dbContext.SaveChanges();
            return Ok(employee);
        }
    }
}
