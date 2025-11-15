using Partner.BrasilConnect.Did.Api.Enum;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;

namespace Partner.BrasilConnect.Did.Api.Models;

public class DidActivation
{
    [Key]
    public int Id { get; set; }
    [Required(ErrorMessage = "Campo DidNumber obrigatorio.")]
    public string DidNumber { get; set; }
    public DidStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
