using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee.Api.Model;

[Table("salaryTbl")]
public class SalaryModel
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SalaryId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]   // ✅ fixes all decimal warnings
    public decimal BasicSalary { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal HRA { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DA { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Bonus { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Deductions { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal NetSalary { get; set; }

    [Required]
    public int Month { get; set; }

    [Required]
    public int Year { get; set; }

    public string? Remarks { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    [ForeignKey("EmployeeId")]
    public EmployeeModel? Employee { get; set; }
}