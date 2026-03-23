using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee.Api.Model;

[Table("attendanceTbl")]
public class AttendanceModel
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AttendanceId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    public DateTime Date { get; set; }

    public TimeSpan? CheckIn { get; set; }

    public TimeSpan? CheckOut { get; set; }

    // Present | Absent | HalfDay | Leave
    public string Status { get; set; } = "Present";

    public string? Notes { get; set; }

    // computed in C#
    public double? WorkingHours { get; set; }

    [ForeignKey("EmployeeId")]
    public EmployeeModel? Employee { get; set; }
}