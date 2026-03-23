using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee.Api.Model;

[Table("departmentTbl")]
public class Department
{
    public int DepartmentId { get; set; }
    [Required, MaxLength(50)]
    public string DepartmentName { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}