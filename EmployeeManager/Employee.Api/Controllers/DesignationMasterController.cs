using Employee.Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Employee.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DesignationMasterController : ControllerBase
{
    private readonly EmployeeDbContext dbContext;

    public DesignationMasterController(EmployeeDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    // GET api/DesignationMaster
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await dbContext.Designations.ToListAsync();
        return Ok(data);
    }

    // GET api/DesignationMaster/1
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDesignationById(int id)
    {
        var designation = await dbContext.Designations.FindAsync(id);
        if (designation == null)
            return NotFound("Designation not found");
        return Ok(designation);
    }

    // POST api/DesignationMaster
    [HttpPost]
    public async Task<IActionResult> AddDesignation(Designation designation)
    {
        bool exists = await dbContext.Designations
            .AnyAsync(d => d.DesignationName.ToLower() == designation.DesignationName.ToLower());

        if (exists)
            return BadRequest("Designation name must be unique.");

        dbContext.Designations.Add(designation);
        await dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetDesignationById),
               new { id = designation.DesignationId }, designation);
    }

    // PUT api/DesignationMaster/1
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDesignation(int id, Designation designation)
    {
        var existing = await dbContext.Designations.FindAsync(id);
        if (existing == null)
            return NotFound("Designation not found");

        existing.DesignationName = designation.DesignationName;
        existing.DepartmentId = designation.DepartmentId;
        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/DesignationMaster/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDesignation(int id)
    {
        var designation = await dbContext.Designations.FindAsync(id);
        if (designation == null)
            return NotFound("Designation not found");

        dbContext.Designations.Remove(designation);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
}