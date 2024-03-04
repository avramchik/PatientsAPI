using PatientsAPI.Database;
using PatientsAPI.Interfaces;
using PatientsAPI.Models;

namespace PatientsAPI.Services;

public class PatientService : IPatientRepository, IDisposable
{
    private readonly PatientContext _context;
    public PatientService(PatientContext context) => _context = context;

    public async Task<List<Patient>> GetAllPatients() =>
        await Task.FromResult(_context.Patients.ToList());

    public async Task<List<Patient>> GetPatientsBy(Func<Patient, bool> predicate) =>
        await Task.FromResult(_context.Patients.Where(predicate).ToList());

    public async Task<Patient?> GetPatientDetail(Guid id) =>
        await _context.Patients.FindAsync(id) ?? null;

    public async Task<Patient?> CreatePatient(Patient model)
    {
        _context.Patients.Add(model);
        await _context.SaveChangesAsync();
        return await GetPatientDetail(model.Id) ?? null;
    }

    public async Task<Patient?> UpdatePatient(Patient model)
    {
        var patient = await GetPatientDetail(model.Id);
        if (patient == null)
            return null;
        _context.Update(model);
        await _context.SaveChangesAsync();
        return model;
    }

    public async Task<Patient?> DeletePatient(Guid id)
    {
        var patient = await GetPatientDetail(id);
        if (patient == null)
            return null;
        _context.Remove(patient);
        await _context.SaveChangesAsync();
        return patient;
    }

    private bool disposed = false;
    public virtual void Dispose(bool disposing)
    {
        if (disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
        disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
