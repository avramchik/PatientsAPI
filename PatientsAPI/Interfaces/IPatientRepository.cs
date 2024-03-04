using PatientsAPI.Models;

namespace PatientsAPI.Interfaces;

public interface IPatientRepository
{
    Task<List<Patient>> GetAllPatients();
    Task<List<Patient>> GetPatientsBy(Func<Patient, bool> predicate);
    Task<Patient?> GetPatientDetail(Guid id);
    Task<Patient?> CreatePatient(Patient model);
    Task<Patient?> UpdatePatient(Patient model);
    Task<Patient?> DeletePatient(Guid id);
}
