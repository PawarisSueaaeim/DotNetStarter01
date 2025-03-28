using System.Globalization;
using WebApplication6.Data;
using WebApplication6.Models;

public class EmployeeService
{
    private readonly ApplicationDbContext _dbContext;

    public EmployeeService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public bool IsDuplicateEmployee(string name, string email)
    {
        return _dbContext.Employees.Any((employee) => employee.Name == name || employee.Email == email);
    }

    public List<UpdateEmployeeDto> EmployeeHighSalary(decimal salary)
    {
        return _dbContext.Employees.Where(employee => employee.Salary > salary).ToList();
    }
}