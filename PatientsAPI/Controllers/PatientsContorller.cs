using AutoMapper;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using PatientsAPI.Interfaces;
using PatientsAPI.Models;
using PatientsAPI.Models.Dto;
using System.Text.RegularExpressions;

namespace PatientsAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientController : ControllerBase
{

    private readonly ILogger<PatientController> _logger;
    private readonly IMapper _mapper;
    private readonly IPatientRepository _patientService;

    public PatientController(IPatientRepository patientService, ILogger<PatientController> logger, IMapper mapper)
    {
        _logger = logger;
        _mapper = mapper;
        _patientService = patientService;
    }

    /// <summary>
    /// Gets all patients list or part of it specified by date.
    /// </summary>
    /// <param name="date"></param>
    /// <returns>Patients list</returns>
    /// <response code="200">All patients, all patients with specified birthDate or empty array.</response>
    [HttpGet]
    public async Task<ActionResult<List<PatientDto>>> GetPatients([FromQuery(Name = "date")] string[] date)
    {
        var data = new List<Patient>();
        if (date.Length > 0)
        {
            var predicate = PredicateBuilder.New<Patient>(false);

            foreach (var dateWithPrefix in date)
            {
                try
                {
                    (var prefix, var dateTime) = ParseDateWithPrefix(dateWithPrefix);
                    var isTimeIncluded = dateWithPrefix.IndexOf('T') > 0;

                    predicate = (prefix, isTimeIncluded) switch
                    {
                        ("eq", false) => predicate.And(p => p.BirthDate.Value.Date.Equals(dateTime.Date)),
                        ("eq", true) => predicate.And(p => p.BirthDate.Equals(dateTime)),
                        ("ne", false) => predicate.And(p => !p.BirthDate.Value.Date.Equals(dateTime.Date)),
                        ("ne", true) => predicate.And(p => !p.BirthDate.Equals(dateTime)),
                        ("gt", false) => predicate.And(p => p.BirthDate.Value.Date > dateTime.Date),
                        ("gt", true) => predicate.And(p => p.BirthDate > dateTime),
                        ("lt", false) => predicate.And(p => p.BirthDate.Value.Date < dateTime.Date),
                        ("lt", true) => predicate.And(p => p.BirthDate < dateTime),
                        ("ge", false) => predicate.And(p => p.BirthDate.Value.Date >= dateTime.Date),
                        ("ge", true) => predicate.And(p => p.BirthDate >= dateTime),
                        ("le", false) => predicate.And(p => p.BirthDate.Value.Date <= dateTime.Date),
                        ("le", true) => predicate.And(p => p.BirthDate <= dateTime),
                        ("sa", _) => predicate.And(p => p.BirthDate.Value.Date < dateTime.Date),
                        ("eb", _) => predicate.And(p => p.BirthDate.Value.Date > dateTime.Date),
                        ("ap", _) => predicate.And(p => p.BirthDate.Value.Subtract(dateTime) <= TimeSpan.FromHours(12)),
                        _ => throw new NotImplementedException($"Not available prefix '{prefix}'. Available prefixes: https://www.hl7.org/fhir/search.html#prefix")
                    };
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            data = await _patientService.GetPatientsBy(predicate.Compile());
        }
        else
        {
            data = await _patientService.GetAllPatients();
        }

        return Ok(_mapper.Map<List<PatientDto>>(data));
    }

    private static (string, DateTime) ParseDateWithPrefix(string dateWithPrefix)
    {
        var prefixReg = new Regex(@"^[a-zA-Z]{2}", RegexOptions.Compiled);

        var prefix = prefixReg.IsMatch(dateWithPrefix) ? prefixReg.Match(dateWithPrefix).Value : throw new Exception("Invalid or missing prefix.");
        var date = dateWithPrefix.Replace(prefix, string.Empty) ?? throw new Exception("Invalid date time query.");

        var isTimeInclude = date.IndexOf('T') > 0;
        var timeOnly = isTimeInclude ? TimeOnly.Parse(date[(date.IndexOf('T') + 1)..]) : TimeOnly.MinValue;
        var dateOnly = isTimeInclude ? DateOnly.Parse(date[..date.IndexOf('T')]) : DateOnly.Parse(date);

        return (prefix, dateOnly.ToDateTime(timeOnly));
    }

    /// <summary>
    /// Gets patient info by id.
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200">Patient info.</response>
    /// <response code="404">Patient was not found.</response>

    [HttpGet("{id}")]
    public async Task<ActionResult<PatientDto>> GetPatientDetail(Guid id)
    {
        try
        {
            var patient = await _patientService.GetPatientDetail(id);

            if (patient == null)
            {
                return NotFound($"Patient with id '{id}' was not found.");
            }

            return Ok(_mapper.Map<PatientDto>(patient));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Creates new patient.
    /// </summary>
    /// <param name="model"></param>
    /// <response code="200">Patient successfully created.</response>
    [HttpPost]
    public async Task<ActionResult<PatientDto>> Create([FromBody] PatientDto model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var patient = await _patientService.CreatePatient(_mapper.Map<Patient>(model));
                return Ok(_mapper.Map<PatientDto>(patient));
            }
            else return BadRequest(ModelState.ValidationState);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Updates patient by id.
    /// </summary>
    /// <param name="model"></param>
    /// <response code="200">Patient successfully updated.</response>
    /// <response code="404">Patient was not found.</response>
    /// <returns></returns>
    [HttpPut]
    public async Task<ActionResult<PatientDto>> Update(PatientDto model)
    {
        try
        {
            var patient = await _patientService.UpdatePatient(_mapper.Map<Patient>(model));
            if (patient == null)
            {
                return NotFound($"Patient with id '{model.Name.Id}' was not found.");
            }

            return Ok(_mapper.Map<PatientDto>(patient));
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Deletes patient by id.
    /// </summary>
    /// <param name="id"></param>
    /// <response code="204">Patient successfully deleted.</response>
    /// <response code="404">Patient was no found.</response>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var patient = await _patientService.DeletePatient(id);
            if (patient == null)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
