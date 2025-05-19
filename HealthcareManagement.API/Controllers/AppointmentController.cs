using Asp.Versioning;
using HealthcareManagement.Application.Appointments.Commands.AddPrescription;
using HealthcareManagement.Application.Appointments.Commands.CancelAppointment;
using HealthcareManagement.Application.Appointments.Commands.Complete;
using HealthcareManagement.Application.Appointments.Commands.Create;
using HealthcareManagement.Application.Appointments.Commands.RescheduleAppointment;
using HealthcareManagement.Application.Appointments.Commands.UpdatePrescription;
using HealthcareManagement.Application.Appointments.Queries.GetAppointmentById;
using HealthcareManagement.Application.Appointments.Queries.GetAppointments;
using HealthcareManagement.Application.Appointments.Queries.GetAppointmentWithPrescription;
using HealthcareManagement.Application.DTOs.Appointment.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareManagement.API.Controllers;

/// <summary>
/// Controller responsible for managing medical appointments
/// </summary>
[Route("api/v{version:apiVersion}/appointments")]
[ApiVersion("1.0")]
[ApiController]
public class AppointmentController : ControllerBase
{
    private readonly ISender _sender;

    public AppointmentController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets a paginated list of appointments
    /// </summary>
    /// <param name="pageNumber"> Page Number </param>
    /// <param name="pageSize"> Page Size</param>
    /// <returns> Paginated list of appointments </returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAppointments([FromQuery]int pageNumber = 1, [FromQuery]int pageSize = 25)
    {
        var appointments = await _sender.Send(new GetAppointmentsQuery(pageNumber, pageSize));

        return Ok(appointments);
    }

    /// <summary>
    /// Gets an appointment by its identifier
    /// </summary>
    /// <param name="id"> Appointment identifier</param>
    /// <returns> Appointment details </returns>
    [HttpGet("{id:Guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointmentById([FromRoute] Guid id)
    {
        var appointment = await _sender.Send(new GetAppointmentByIdQuery(id));

        return Ok(appointment);
    }

    /// <summary>
    /// Gets prescriptions associated with an appointment
    /// </summary>
    /// <param name="id">Appointment identifier</param>
    /// <returns>Appointment with its prescriptions</returns>
    [HttpGet("{id:Guid}/prescriptions")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAppointmentPrescriptions([FromRoute] Guid id)
    {
        var appointment = await _sender.Send(new GetAppointmentWithPrescriptionQuery(id));

        return Ok(appointment);
    }


    /// <summary>
    /// Creates a new appointment
    /// </summary>
    /// <param name="model">Appointment data</param>
    /// <returns>Created appointment</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Post([FromBody] CreateAppointmentDTO model)
    {
        var appointmentId = await _sender.Send(new CreateAppointmentCommand(model.DoctorId, model.PatientId, model.AppointmentDate));

        return CreatedAtAction(nameof(GetAppointmentById), new { id = appointmentId }, appointmentId);
    }

    /// <summary>
    /// Cancels an existing appointment
    /// </summary>
    /// <param name="id">Appointment identifier</param>
    /// <returns>No content response</returns>
    [HttpPost("{id:Guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CancelAppointment([FromRoute] Guid id)
    {
        await _sender.Send(new CancelAppointmentCommand(id));

        return NoContent();
    }

    /// <summary>
    /// Reschedules an existing appointment
    /// </summary>
    /// <param name="id">Appointment identifier</param>
    /// <param name="model">New scheduling data</param>
    /// <returns>No content response</returns>
    [HttpPost("{id:Guid}/reschedule")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RescheduleAppointment([FromRoute] Guid id, [FromBody] RescheduleAppointmentDTO model)
    {
        if (id != model.Id)
        {
            return BadRequest("Route ID does not match the ID in the request body");
        }

        await _sender.Send(new RescheduleAppointmentCommand(model.Id, model.NewAppointmentDate));
        return NoContent();
    }

    /// <summary>
    /// Marks an appointment as completed
    /// </summary>
    /// <param name="id">Appointment identifier</param>
    /// <param name="model">Completion data</param>
    /// <returns>No content response</returns>
    [HttpPost("{id:Guid}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteAppointment([FromRoute] Guid id, [FromBody] CompleteAppointmentDTO model)
    {
        if (id != model.AppointmentId)
        {
            return BadRequest("Route ID does not match the ID in the request body");
        }
        await _sender.Send(new CompleteAppointmentCommand(model.AppointmentId));

        return NoContent();
    }

    /// <summary>
    /// Adds a prescription to an appointment
    /// </summary>
    /// <param name="id">Appointment identifier</param>
    /// <param name="model">Prescription data</param>
    /// <returns>Created prescription</returns>
    [HttpPost("{id:Guid}/prescriptions")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddPrescription([FromRoute] Guid id, [FromBody] AddPrescriptionDTO model)
    {
        if (id != model.AppointmentId)
        {
            return BadRequest("Route ID does not match the ID in the request body");
        }

        var prescriptionId = await _sender.Send(new AddPrescriptionCommand(
                model.AppointmentId, 
                model.Medication, 
                model.Dosage, 
                model.Duration, 
                model.Instructions)
            );

        return CreatedAtAction(
            nameof(GetAppointmentPrescriptions),
            new { id = model.AppointmentId },
            new { PrescriptionId = prescriptionId });
    }

    /// <summary>
    /// Updates an existing prescription
    /// </summary>
    /// <param name="appointmentId">Appointment identifier</param>
    /// <param name="prescriptionId">Prescription identifier</param>
    /// <param name="model">New prescription data</param>
    /// <returns>No content response</returns>
    [HttpPut("{appointmentId:Guid}/prescriptions/{prescriptionId:Guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePrescription([FromRoute] Guid appointmentId, [FromRoute] Guid prescriptionId, [FromBody] UpdatePrescriptionDTO model)
    {
        if (appointmentId != model.AppointmentId || prescriptionId != model.PrescriptionId)
        {
            return BadRequest("Route ID does not match the ID in the request body");
        }

        await _sender.Send(new UpdatePrescriptionCommand(
            model.AppointmentId,
            model.PrescriptionId,
            model.Medication,
            model.Dosage,
            model.Duration,
            model.Instructions
        ));

        return NoContent();
    }
}
