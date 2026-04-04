using System.Text.Json.Serialization;

namespace DoctorBooking.API.Application.DTOs.Review
{
    // Replace the record with a proper class for reliable model binding
    public record CreateReviewDto(
       [property: JsonPropertyName("doctorId")] int DoctorId,
       [property: JsonPropertyName("rating")] int Rating,
       [property: JsonPropertyName("comment")] string? Comment
   );
}
