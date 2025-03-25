using Microsoft.AspNetCore.Authorization;
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
        private readonly ApplicationDbContext _dbContext;
        private readonly EmployeeService _employeeService;
        public EmployeesController(ApplicationDbContext dbContext, EmployeeService employeeService)
        {
            _dbContext = dbContext;
            _employeeService = employeeService;
        }

        [Authorize]
        [HttpGet]
        [Route("get")]
        public IActionResult GetAllEmployees()
        {
            var allEmployees = _dbContext.Employees.ToList();
            return Ok(allEmployees);
        }

        [Authorize]
        [HttpGet]
        [Route("get/{id:guid}")]
        public IActionResult GetEmployeeById(Guid id)
        {
            var employee = _dbContext.Employees.Find(id);

            if (employee is null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [Authorize]
        [HttpPost]
        [Route("add")]
        public IActionResult AddEmployee(AddEmployeeDto addEmployee)
        {
            if (_employeeService.IsDuplicateEmployee(addEmployee.Name, addEmployee.Email))
            {
                return BadRequest("Name or Email already exists!");
            }
            var employee = new Employee()
            {
                Name = addEmployee.Name,
                Email = addEmployee.Email,
                Phone = addEmployee.Phone,
                Salary = addEmployee.Salary,
            };
            _dbContext.Employees.Add(employee);
            _dbContext.SaveChanges();
            return Ok(employee);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPut]
        [Route("update/{id:guid}")]
        public IActionResult UpdateEmployee(Guid id, UpdateEmployeeDto updateEmployee)
        {
            var employee = _dbContext.Employees.Find(id);

            if (employee == null)
            {
                return NotFound();
            }
            employee.Name = updateEmployee.Name;
            employee.Email = updateEmployee.Email;
            employee.Phone = updateEmployee.Phone;
            employee.Salary = updateEmployee.Salary;

            _dbContext.SaveChanges();

            var response = new UpdateEmployeeDto()
            {
                Name = updateEmployee.Name,
                Email = updateEmployee.Email,
                Phone = updateEmployee.Phone,
                Salary = updateEmployee.Salary
            };

            return Ok(response);
        }

        [Authorize]
        [HttpPatch]
        [Route("update-salary/{id:guid}")]
        public IActionResult UpdateSalary(Guid id, UpdateSalaryDto salary)
        {
            var employee = _dbContext.Employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }

            employee.Salary = salary.Salary;
            _dbContext.SaveChanges();
            return Ok(employee);

        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("delete/{id:guid}")]
        public IActionResult DeleteEmployee(Guid id)
        {
            var employee = _dbContext.Employees.Find(id);

            if (employee is null)
            {
                var response = new
                {
                    user_id = id,
                    message = "Employee Not Found",
                };
                return NotFound(response);
            }

            _dbContext.Employees.Remove(employee);
            _dbContext.SaveChanges();

            return Ok("Delete Successfuly!!");
        }
    }
}
