using Asp.Versioning;
using HealthcareManagement.Application.DTOs.Patient.Commands;
using HealthcareManagement.Application.Patients.Commands.CreatePatient;
using HealthcareManagement.Application.Patients.Commands.RemovePatient;
using HealthcareManagement.Application.Patients.Commands.UpdatePatient;
using HealthcareManagement.Application.Patients.Queries.GetPatientById;
using HealthcareManagement.Application.Patients.Queries.GetPatients;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareManagement.API.Controllers;

/// <summary>
/// Controller responsible for managing patients
/// </summary>
[Route("api/v{version:apiVersion}/patients")]
[ApiVersion("1.0")]
[ApiController]
public class PatientController : ControllerBase
{
    private readonly ISender _sender;

    public PatientController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets a paginated list of patients
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Paginated list of patients</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPatients(int pageNumber = 1, int pageSize = 25)
    {
        var patients = await _sender.Send(new GetPatientsQuery(pageNumber, pageSize));

        return Ok(patients);
    }

    /// <summary>
    /// Gets a patient by their identifier
    /// </summary>
    /// <param name="id">Patient identifier</param>
    /// <returns>Patient details</returns>
    [HttpGet("{id:Guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPatientById([FromRoute] Guid id)
    {
        var patient = await _sender.Send(new GetPatientByIdQuery(id));

        return Ok(patient);
    }

    /// <summary>
    /// Creates a new patient
    /// </summary>
    /// <param name="model">Patient data</param>
    /// <returns>Created patient</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Post([FromBody] CreatePatientDTO model)
    {
        var patientId = await _sender.Send(new CreatePatientCommand(
            model.FirstName, 
            model.LastName, 
            model.BirthDate, 
            model.Cpf,
            model.PhoneNumber, 
            model.Email));

        return CreatedAtAction(nameof(GetPatientById), new { id = patientId }, patientId);
    }

    /// <summary>
    /// Updates an existing patient data
    /// </summary>
    /// <param name="id">Patient identifier</param>
    /// <param name="model">New patient data</param>
    /// <returns>No content response</returns>
    [HttpPut("{id:Guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdatePatientDTO model)
    {
        if (id != model.Id)
        {
            return BadRequest("Route ID does not match the ID in the request body");
        }

        await _sender.Send(new UpdatePatientCommand(
            model.Id, 
            model.FirstName,
            model.LastName,
            model.BirthDate,
            model.Cpf,
            model.PhoneNumber,
            model.Email));

        return NoContent();
    }

    /// <summary>
    /// Removes an existing patient
    /// </summary>
    /// <param name="id">Patient identifier</param>
    /// <returns>No content response</returns>
    [HttpDelete("{id:Guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _sender.Send(new RemovePatientCommand(id));

        return NoContent();
    }
}
