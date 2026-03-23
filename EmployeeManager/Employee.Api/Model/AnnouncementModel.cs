using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee.Api.Model;

[Table("announcementTbl")]
public class AnnouncementModel
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int AnnouncementId { get; set; }

    [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    // All | HR | Employee
    public string TargetRole { get; set; } = "All";

    public DateTime CreatedDate { get; set; } = DateTime.Now;

    public bool IsActive { get; set; } = true;
}