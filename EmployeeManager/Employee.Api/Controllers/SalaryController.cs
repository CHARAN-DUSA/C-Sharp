using Employee.Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Employee.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SalaryController : ControllerBase
{
    private readonly EmployeeDbContext _db;
    public SalaryController(EmployeeDbContext db) => _db = db;

    // GET api/Salary — all salary records (HR)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await (
            from s in _db.Salaries
            join e in _db.Employees on s.EmployeeId equals e.EmployeeId
            select new
            {
                s.SalaryId,
                s.EmployeeId,
                EmployeeName = e.Name,
                e.DesignationId,
                s.BasicSalary,
                s.HRA,
                s.DA,
                s.Bonus,
                s.Deductions,
                s.NetSalary,
                s.Month,
                s.Year,
                s.Remarks,
                s.CreatedDate
            }
        ).OrderByDescending(s => s.Year)
         .ThenByDescending(s => s.Month)
         .ToListAsync();

        return Ok(data);
    }

    // GET api/Salary/employee/5 — salary for one employee (employee self-view)
    [HttpGet("employee/{employeeId}")]
    public async Task<IActionResult> GetByEmployee(int employeeId)
    {
        var data = await _db.Salaries
            .Where(s => s.EmployeeId == employeeId)
            .OrderByDescending(s => s.Year)
            .ThenByDescending(s => s.Month)
            .ToListAsync();

        return Ok(data);
    }

    // GET api/Salary/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var salary = await _db.Salaries.FindAsync(id);
        if (salary == null) return NotFound("Salary record not found.");
        return Ok(salary);
    }

    // POST api/Salary
    [HttpPost]
    public async Task<IActionResult> Create(SalaryModel salary)
    {
        // prevent duplicate month/year for same employee
        bool exists = await _db.Salaries.AnyAsync(s =>
            s.EmployeeId == salary.EmployeeId &&
            s.Month == salary.Month &&
            s.Year == salary.Year);

        if (exists)
            return BadRequest("Salary for this employee and month already exists.");

        salary.NetSalary = salary.BasicSalary + salary.HRA +
                           salary.DA + salary.Bonus - salary.Deductions;
        salary.CreatedDate = DateTime.Now;

        _db.Salaries.Add(salary);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = salary.SalaryId }, salary);
    }

    // PUT api/Salary/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, SalaryModel salary)
    {
        var existing = await _db.Salaries.FindAsync(id);
        if (existing == null) return NotFound("Salary record not found.");

        existing.BasicSalary = salary.BasicSalary;
        existing.HRA = salary.HRA;
        existing.DA = salary.DA;
        existing.Bonus = salary.Bonus;
        existing.Deductions = salary.Deductions;
        existing.NetSalary = salary.BasicSalary + salary.HRA +
                               salary.DA + salary.Bonus - salary.Deductions;
        existing.Month = salary.Month;
        existing.Year = salary.Year;
        existing.Remarks = salary.Remarks;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE api/Salary/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var salary = await _db.Salaries.FindAsync(id);
        if (salary == null) return NotFound("Salary record not found.");
        _db.Salaries.Remove(salary);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}