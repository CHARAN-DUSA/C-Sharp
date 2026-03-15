using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos
{
    public record UpdateGameDto
    (
        [Required][StringLength(50)] string Name,
        [Range(1,100)] int GenreId,
        [Range(1,100)]decimal Price,
        DateOnly ReleaseDate
    );
}



//Does the object need to change after creation?

//Yes → use class
//No, just passing data around → use record

//Always use record for DTOs because:

//A DTO just carries data from client to API — it doesn't need to change
//Less code to write — properties are auto created from the ()
//Cleaner and more readable