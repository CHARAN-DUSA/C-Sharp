using Employee.Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Employee.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeMasterController : ControllerBase
{
    private readonly EmployeeDbContext dbContext;

    public EmployeeMasterController(EmployeeDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    // GET api/EmployeeMaster
    [HttpGet]
    public async Task<IActionResult> GetAllEmployees()
    {
        try
        {
            var data = await (
                from emp in dbContext.Employees
                join des in dbContext.Designations
                    on emp.DesignationId equals des.DesignationId into desGroup
                from des in desGroup.DefaultIfEmpty()
                join dept in dbContext.Departments
                    on des.DepartmentId equals dept.DepartmentId into deptGroup
                from dept in deptGroup.DefaultIfEmpty()
                select new
                {
                    emp.EmployeeId,
                    emp.Name,
                    emp.ContactNo,
                    emp.Email,
                    emp.City,
                    emp.State,
                    emp.Pincode,
                    emp.AltContactNo,
                    emp.Address,
                    emp.DesignationId,
                    DesignationName = des != null ? des.DesignationName : "",
                    DepartmentId = dept != null ? dept.DepartmentId : 0,
                    DepartmentName = dept != null ? dept.DepartmentName : "",
                    emp.Role,
                    emp.CreatedDate,
                    emp.ModifiedDate
                }
            ).ToListAsync();

            return Ok(data);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    // GET api/EmployeeMaster/filter
    [HttpGet("filter")]
    public async Task<IActionResult> GetEmployees(
        [FromQuery] string? search,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortOrder,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        var query = dbContext.Employees.AsQueryable();

        // 🔍 Filter by search (name, city, state)
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(e =>
                e.Name.ToLower().Contains(search.ToLower()) ||
                e.City.ToLower().Contains(search.ToLower()) ||
                e.State.ToLower().Contains(search.ToLower()));
        }

        // 🔃 Sort
        query = sortBy?.ToLower() switch
        {
            "name" => sortOrder == "desc" ? query.OrderByDescending(e => e.Name)
                                             : query.OrderBy(e => e.Name),
            "city" => sortOrder == "desc" ? query.OrderByDescending(e => e.City)
                                             : query.OrderBy(e => e.City),
            "created" => sortOrder == "desc" ? query.OrderByDescending(e => e.CreatedDate)
                                             : query.OrderBy(e => e.CreatedDate),
            _ => query.OrderBy(e => e.EmployeeId) // default sort
        };

        // 📄 Pagination
        var totalRecords = await query.CountAsync();
        var employees = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(new
        {
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
            Data = employees
        });
    }

    // GET api/EmployeeMaster/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployeeById(int id)
    {
        var employee = await dbContext.Employees.FindAsync(id);
        if (employee == null)
            return NotFound("Employee not found");
        return Ok(employee);
    }

    // POST api/EmployeeMaster
    [HttpPost]
    public async Task<IActionResult> AddEmployee(EmployeeModel employee)
    {
        // ✅ Unique contact check
        bool contactExists = await dbContext.Employees
            .AnyAsync(e => e.ContactNo == employee.ContactNo);
        if (contactExists)
            return BadRequest("Contact number already exists.");

        // ✅ Unique email check
        bool emailExists = await dbContext.Employees
            .AnyAsync(e => e.Email.ToLower() == employee.Email.ToLower());
        if (emailExists)
            return BadRequest("Email already exists.");

        employee.CreatedDate = DateTime.Now;
        employee.ModifiedDate = DateTime.Now;

        dbContext.Employees.Add(employee);
        await dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetEmployeeById),
               new { id = employee.EmployeeId }, employee);
    }

    // PUT api/EmployeeMaster/1
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, EmployeeModel employee)
    {
        var existing = await dbContext.Employees.FindAsync(id);
        if (existing == null)
            return NotFound("Employee not found");

        // ✅ Unique contact check (exclude current employee)
        bool contactExists = await dbContext.Employees
            .AnyAsync(e => e.ContactNo == employee.ContactNo && e.EmployeeId != id);
        if (contactExists)
            return BadRequest("Contact number already exists.");

        // ✅ Unique email check (exclude current employee)
        bool emailExists = await dbContext.Employees
            .AnyAsync(e => e.Email.ToLower() == employee.Email.ToLower() && e.EmployeeId != id);
        if (emailExists)
            return BadRequest("Email already exists.");

        existing.Name = employee.Name;
        existing.ContactNo = employee.ContactNo;
        existing.Email = employee.Email;
        existing.City = employee.City;
        existing.State = employee.State;
        existing.Pincode = employee.Pincode;
        existing.AltContactNo = employee.AltContactNo;
        existing.DesignationName = employee.DesignationName;
        existing.Address = employee.Address;
        existing.DesignationId = employee.DesignationId;
        existing.ModifiedDate = DateTime.Now; // ✅ auto update modified date

        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/EmployeeMaster/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await dbContext.Employees.FindAsync(id);
        if (employee == null)
            return NotFound("Employee not found");

        dbContext.Employees.Remove(employee);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto login)
    {
        // ✅ Find by Email AND ContactNo together
        var user = await dbContext.Employees
            .FirstOrDefaultAsync(e =>
                e.Email.ToLower() == login.Email!.ToLower() &&
                e.ContactNo == login.ContactNo);

        if (user == null)
            return Unauthorized("Invalid email or contact number.");

        return Ok(new 
        {
            Message = "Login Successful",
            data = new
            {
                 user.EmployeeId,
                 user.Name,
                 user.Email,
                 user.DesignationId,
                 user.Role
            }
            
        });
    }
}
