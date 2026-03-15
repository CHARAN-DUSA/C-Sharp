namespace GameStore.Api.Dtos;

public record  GameSummaryDto
    (
    int Id,
    string Name,
    string Genre,
    int GenreId,
    decimal Price,
    DateOnly ReleaseDate
    );