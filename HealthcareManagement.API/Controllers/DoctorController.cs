using Asp.Versioning;
using HealthcareManagement.Application.Doctors.Commands.AddSpecialty;
using HealthcareManagement.Application.Doctors.Commands.CreateDoctor;
using HealthcareManagement.Application.Doctors.Commands.RemoveDoctor;
using HealthcareManagement.Application.Doctors.Commands.RemoveSpecialty;
using HealthcareManagement.Application.Doctors.Commands.UpdateDoctor;
using HealthcareManagement.Application.Doctors.Queries.GetDoctorById;
using HealthcareManagement.Application.Doctors.Queries.GetDoctors;
using HealthcareManagement.Application.DTOs.Doctor.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareManagement.API.Controllers;

/// <summary>
/// Controller responsible for managing doctors
/// </summary>
[Route("api/v{version:apiVersion}/doctors")]
[ApiVersion("1.0")]
[ApiController]
public class DoctorController : ControllerBase
{
    private readonly ISender _sender;

    public DoctorController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets a paginated list of doctors
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Paginated list of doctors</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery]int pageNumber = 1, [FromQuery]int pageSize = 25)
    {
        var doctors = await _sender.Send(new GetDoctorsQuery(pageNumber, pageSize));

        return Ok(doctors);
    }

    /// <summary>
    /// Gets a doctor by their identifier
    /// </summary>
    /// <param name="id">Doctor identifier</param>
    /// <returns>Doctor details</returns>
    [HttpGet("{id:Guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id)
    {
        var doctor = await _sender.Send(new GetDoctorByIdQuery(id));

        return Ok(doctor);
    }

    /// <summary>
    /// Creates a new doctor
    /// </summary>
    /// <param name="model">Doctor data</param>
    /// <returns>Created doctor</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Post([FromBody] CreateDoctorDTO model)
    {
        var doctorId = await _sender.Send(new CreateDoctorCommand(
            model.FirstName, 
            model.LastName, 
            model.BirthDate,
            model.Cpf,
            model.PhoneNumber, 
            model.Email));

        return CreatedAtAction(nameof(GetById), new { id = doctorId }, doctorId);
    }

    /// <summary>
    /// Adds a specialty to a doctor
    /// </summary>
    /// <param name="id">Doctor identifier</param>
    /// <param name="model">Specialty data</param>
    /// <returns>No content response</returns>
    [HttpPost("{id:Guid}/specialties")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddSpecialty([FromRoute] Guid id,[FromBody] AddSpecialtyToDoctorDTO model)
    {
        if(id != model.DoctorId)
            return BadRequest("Route ID does not match the ID in the request body");

        await _sender.Send(new AddSpecialtyToDoctorCommand(model.DoctorId, model.Specialty));

        return NoContent();
    }

    /// <summary>
    /// Removes a specialty from a doctor
    /// </summary>
    /// <param name="doctorId">Doctor identifier</param>
    /// <param name="model">Specialty data to be removed</param>
    /// <returns>No content response</returns>
    [HttpDelete("{doctorId:Guid}/specialties")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveSpecialty([FromRoute] Guid doctorId, [FromBody] RemoveSpecialtyFromDoctorDTO model)
    {
        if (doctorId != model.DoctorId)
            return BadRequest("Route ID does not match the ID in the request body");

        await _sender.Send(new RemoveSpecialtyFromDoctorCommand(model.DoctorId, model.Specialty));

        return NoContent();
    }

    /// <summary>
    /// Updates an existing doctor's data
    /// </summary>
    /// <param name="id">Doctor identifier</param>
    /// <param name="model">New doctor data</param>
    /// <returns>No content response</returns>
    [HttpPut("{id:Guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateDoctorDTO model)
    {
        if (id != model.Id)
            return BadRequest("Route ID does not match the ID in the request body");

        await _sender.Send(new UpdateDoctorCommand(
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
    /// Removes an existing doctor
    /// </summary>
    /// <param name="id">Doctor identifier</param>
    /// <returns>No content response</returns>
    [HttpDelete("{id:Guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await _sender.Send(new RemoveDoctorCommand(id));

        return NoContent();
    }
}
