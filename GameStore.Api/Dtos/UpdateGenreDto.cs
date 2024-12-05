using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record class UpdateGenreDto([Required] [StringLength(30)] string Name);
