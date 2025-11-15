using System.ComponentModel.DataAnnotations;

namespace Partner.BrasilConnect.Did.Api.DTO;

public record DidCreationDto
{
    [Required(ErrorMessage = "Campo DidNumber obrigatorio.")]
    public string DidNumber { get; init; } = default!;
}
