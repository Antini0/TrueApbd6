using System.ComponentModel.DataAnnotations;

namespace proba5.Models.DTOs;

public class AddAnimal
{
    [Required]
    [MinLength(1)]
    [MaxLength(200)]
    public string name { get; set; }
    [MaxLength(200)]
    public string? description { get; set; }
}