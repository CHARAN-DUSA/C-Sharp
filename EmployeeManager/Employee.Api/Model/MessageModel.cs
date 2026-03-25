using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employee.Api.Model;

[Table("messageTbl")]
public class MessageModel
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MessageId { get; set; }

    [Required]
    public int SenderId { get; set; }

    public int? ReceiverId { get; set; } // NULL = send to all

    [Required]
    [MaxLength(1000)]
    public string MessageText { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // 🔗 Navigation Properties
    [ForeignKey("SenderId")]
    public EmployeeModel? Sender { get; set; }

    [ForeignKey("ReceiverId")]
    public EmployeeModel? Receiver { get; set; }
    public string? FileUrl { get; set; } 
}