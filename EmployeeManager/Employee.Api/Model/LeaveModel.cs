using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee.Api.Model;

[Table("leaveTbl")]
public class LeaveModel
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int LeaveId { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    // Sick | Casual | Earned | Unpaid
    [Required, MaxLength(20)]
    public string LeaveType { get; set; } = "Casual";

    [Required]
    public DateTime FromDate { get; set; }

    [Required]
    public DateTime ToDate { get; set; }

    public int TotalDays { get; set; }

    [Required, MaxLength(300)]
    public string Reason { get; set; } = string.Empty;

    // Pending | Approved | Rejected
    public string Status { get; set; } = "Pending";

    public string? RejectionReason { get; set; }

    public int? ApprovedByEmployeeId { get; set; }

    public DateTime AppliedDate { get; set; } = DateTime.Now;

    public DateTime? ActionDate { get; set; }

    [ForeignKey("EmployeeId")]
    public EmployeeModel? Employee { get; set; }
}