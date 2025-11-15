using Partner.BrasilConnect.Did.Api.Enum;
using System.ComponentModel.DataAnnotations;

namespace Partner.BrasilConnect.Did.Api.DTO;

public record DidStatusUpdateDto
{
    [Required(ErrorMessage = "Campo Status obrigatorio.")]
    public DidStatus Status { get; init; }
    public string? ErrorMessage { get; init; }
}
