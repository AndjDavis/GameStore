using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record class CreateGenreDto([Required] [StringLength(30)] string Name);
