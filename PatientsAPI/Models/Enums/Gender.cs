using System.Text.Json.Serialization;

namespace PatientsAPI.Models.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Gender
{
    Unknown,
    Male,
    Female,
    Other
}
