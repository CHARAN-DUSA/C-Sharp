using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee.Api.Model;

[Table("designationTbl")]
public class Designation
{
    public int DesignationId { get; set; }

    public int DepartmentId { get; set; }

    [Required, MaxLength(50)]
    public string DesignationName { get; set; } = string.Empty;

}