using HealthcareManagement.Domain.Exceptions.Base;
using HealthcareManagement.Domain.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HealthcareManagement.API.Filters;

public class ExceptionFilter : IExceptionFilter
{
    private readonly Dictionary<Type, Func<Exception, IActionResult>> _exceptionHandlers;

    public ExceptionFilter()
    {
        _exceptionHandlers = new Dictionary<Type, Func<Exception, IActionResult>>
        {
            { typeof(FluentValidation.ValidationException), HandleValidationException },
            { typeof(DomainValidationException), ex => new BadRequestObjectResult(new { Error = ex.Message }) },
            { typeof(NotFoundException), ex => new NotFoundObjectResult(new { Error = ex.Message }) },
            { typeof(ConflictException), ex => new ConflictObjectResult(new { Error = ex.Message }) },
            { typeof(ArgumentNullException), _ => new NotFoundObjectResult(new { Error = "Resource not found." }) },
            { typeof(KeyNotFoundException), _ => new NotFoundObjectResult(new { Error = "Resource not found." }) },
            { typeof(HttpRequestException), _ => new StatusCodeResult(StatusCodes.Status500InternalServerError) },
            { typeof(InvalidOperationException), _ => new StatusCodeResult(StatusCodes.Status500InternalServerError) }
        };
    }

    public void OnException(ExceptionContext context)
    {
        var exceptionType = context.Exception.GetType();

        if (_exceptionHandlers.TryGetValue(exceptionType, out var handler))
        {
            context.Result = handler(context.Exception);
            context.ExceptionHandled = true;
            return;
        }

        foreach (var handlerPair in _exceptionHandlers)
        {
            if (handlerPair.Key.IsAssignableFrom(exceptionType))
            {
                context.Result = handlerPair.Value(context.Exception);
                context.ExceptionHandled = true;
                return;
            }
        }
    }

    private IActionResult HandleValidationException(Exception exception)
    {
        var validationException = (FluentValidation.ValidationException)exception;
        var errors = validationException.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray()
            );

        return new ObjectResult(new { Errors = errors })
        {
            StatusCode = 400
        };
    }
}
