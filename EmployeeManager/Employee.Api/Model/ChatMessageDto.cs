public class ChatMessageDto
{
    public int MessageId { get; set; }
    public int SenderId { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public int? ReceiverId { get; set; }

    public string? MessageText { get; set; }
    public string? FileUrl { get; set; }   // 🔥 NEW

    public DateTime CreatedDate { get; set; }
}