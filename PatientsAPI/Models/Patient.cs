using PatientsAPI.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientsAPI.Models;

public class Patient
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string Use { get; set; }
    [Required]
    public string Family { get; set; }
    public string Given { get; set; }
    public Gender Gender { get; set; } = Gender.Unknown;
    [Required]
    public DateTime? BirthDate { get; set; }
    public bool Active { get; set; } = true;
}
