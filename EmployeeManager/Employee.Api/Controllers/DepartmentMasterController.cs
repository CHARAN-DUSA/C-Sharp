using Employee.Api.Model;
using Microsoft.AspNetCore.Mvc;

namespace Employee.Api.Controllers;

[Route("api/[controller]")]
[ApiController] // By using this we explicitly don't need to write FromBody and [ApiController] enables automatic model validation, error handling etc.
public class DepartmentMasterController : ControllerBase   // ControllerBase Gives you helper methods like Ok(), NotFound(), BadRequest() etc
{
    private readonly EmployeeDbContext dbContext;

    public DepartmentMasterController(EmployeeDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet()]
    public IActionResult GetDepartment()
    {
        var deptList = dbContext.Departments.ToList(); // ✅ Designation table
        return Ok(deptList);
    }

    // GET api/DepartmentMaster/1
    [HttpGet("{id}")]
    public IActionResult GetDepartmentById(int id)
    {
        var dept = dbContext.Departments.Find(id);
        if (dept == null)
            return NotFound("Department not found");
        return Ok(dept);
    }

    // POST api/DepartmentMaster
    [HttpPost]
    public IActionResult AddDepartment(Department dept)
    {
        bool exists = dbContext.Departments
            .Any(d => d.DepartmentName.ToLower() == dept.DepartmentName.ToLower());

        if (exists)
            return BadRequest("Department name must be unique.");

        dbContext.Departments.Add(dept);
        dbContext.SaveChanges();
        return CreatedAtAction(nameof(GetDepartmentById),
               new { id = dept.DepartmentId }, dept); // ✅ 201 Created
    }

    // PUT api/DepartmentMaster/1
    [HttpPut("{id}")]
    public IActionResult UpdateDepartment(int id, Department dept)
    {
        var existing = dbContext.Departments.Find(id);
        if (existing == null)
            return NotFound("Department not found");

        existing.DepartmentName = dept.DepartmentName;
        existing.IsActive = dept.IsActive;
        dbContext.SaveChanges();
        return Ok(existing); // ✅ 204 standard for updates
    }

    // DELETE api/DepartmentMaster/1
    [HttpDelete("{id}")]
    public IActionResult DeleteDepartment(int id)
    {
        var dept = dbContext.Departments.Find(id);
        if (dept == null)
            return NotFound("Department not found");

        dbContext.Departments.Remove(dept);
        dbContext.SaveChanges();
        return NoContent();

    }
}