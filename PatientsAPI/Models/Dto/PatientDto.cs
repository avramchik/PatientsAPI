using PatientsAPI.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace PatientsAPI.Models.Dto;

public class PatientDto
{
    public Name Name { get; set; } = new Name();
    public Gender Gender { get; set; }
    [Required]
    public DateTime BirthDate { get; set; }
    public bool Active { get; set; } = true;
    public PatientDto() { }
}

public class Name
{
    public Guid Id { get; set; }
    public string Use { get; set; } = string.Empty;
    [Required]
    public string Family { get; set; }
    public string[] Given { get; set; } = Array.Empty<string>();
}