using Employee.Api.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("taskTbl")]
public class TaskModel
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TaskId { get; set; }

    [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public int AssignedToEmployeeId { get; set; }

    public int? AssignedByEmployeeId { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    public string Status { get; set; } = "Pending";
    public string Priority { get; set; } = "Medium";

    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime? CompletedDate { get; set; }

    // ✅ Employee Note
    public string? CompletionNote { get; set; }

    // ✅ HR Message (NEW)
    public string? HrMessage { get; set; }

    [ForeignKey("AssignedToEmployeeId")]
    public EmployeeModel? AssignedTo { get; set; }
}