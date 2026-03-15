using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos
{
    public record UpdateGenreDto(
        [Required][StringLength(50)] string Name
    );
}