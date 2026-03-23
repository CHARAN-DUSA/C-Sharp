using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee.Api.Model;

[Table("employeeTbl")]
public class EmployeeModel
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("employeeId")]
    public int EmployeeId { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(10), MinLength(10)]
    public string ContactNo { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Pincode { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string DesignationName { get; set; } = string.Empty;

    public string? AltContactNo { get; set; }   // ✅ nullable
    public string? Role { get; set; }            // ✅ nullable

    public int DesignationId { get; set; }

    public DateTime? CreatedDate { get; set; }   // ✅ nullable
    public DateTime? ModifiedDate { get; set; }  // ✅ nullable
}
public class LoginDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    public string ContactNo { get; set; } = string.Empty;
}

