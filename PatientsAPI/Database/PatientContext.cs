using Microsoft.EntityFrameworkCore;
using PatientsAPI.Models;

namespace PatientsAPI.Database;

public class PatientContext : DbContext
{
    public PatientContext(DbContextOptions<PatientContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
    public DbSet<Patient> Patients { get; set; }
}
