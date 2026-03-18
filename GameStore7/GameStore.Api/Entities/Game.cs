using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Entities;

public class Game
{
    public int Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public required string Name { get; set; }
    [Required]
    [StringLength(30, MinimumLength = 1)]
    public required string Genre { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
    public decimal Price { get; set; }
    public DateTime ReleaseDate { get; set; }

    [Url]
    [StringLength(100, MinimumLength = 1)]
    public required string ImageUri { get; set; }
    public string? Title { get; internal set; }
}