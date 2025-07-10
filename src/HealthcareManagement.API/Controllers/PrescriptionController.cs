using Asp.Versioning;
using HealthcareManagement.Application.Appointments.Queries.GetAppointmentById;
using HealthcareManagement.Application.Prescriptions.Queries.GetPrescriptionById;
using HealthcareManagement.Application.Prescriptions.Queries.GetPrescriptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HealthcareManagement.API.Controllers;

/// <summary>
/// Controller responsible for managing medical prescriptions
/// </summary>
[Route("api/v{version:apiVersion}/prescriptions")]
[ApiVersion("1.0")]
[ApiController]
public class PrescriptionController : ControllerBase
{
    private readonly ISender _sender;

    public PrescriptionController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Gets a paginated list of prescriptions
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>Paginated list of prescriptions</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get( [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 25)
    {
        var prescriptions = await _sender.Send(new GetPrescriptionsQuery(pageNumber, pageSize));

        return Ok(prescriptions);
    }

    /// <summary>
    /// Gets a prescription by its identifier
    /// </summary>
    /// <param name="id">Prescription identifier</param>
    /// <returns>Prescription details</returns>
    [HttpGet("{id:Guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromRoute] Guid id)
    {
        var prescription = await _sender.Send(new GetPrescriptionByIdQuery(id));

        return Ok(prescription);
    }
}
