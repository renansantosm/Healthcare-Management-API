using Asp.Versioning;
using FluentValidation;
using HealthcareManagement.Application.Services.Interfaces;
using HealthcareManagement.Application.Services.Validation;
using HealthcareManagement.Application.Validators;
using HealthcareManagement.Domain.Interfaces;
using HealthcareManagement.Infra.Data.Context;
using HealthcareManagement.Infra.Data.Providers;
using HealthcareManagement.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HealthcareManagement.Infra.IoC;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
            b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
           );

        // Repository
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IDoctorRepository, DoctorRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();

        // Provider
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // Versioning
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        // Mediator
        var myhandlers = AppDomain.CurrentDomain.Load("HealthcareManagement.Application");
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(myhandlers);
            cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
        });

        // Fluent Validation
        services.AddValidatorsFromAssembly(Assembly.Load("HealthcareManagement.Application"));

        // Validation Services
        services.AddScoped<IDoctorValidationService, DoctorValidationService>();
        services.AddScoped<IAppointmentValidationService, AppointmentValidationService>();
        services.AddScoped<IPatientValidationService, PatientValidationService>();
        services.AddScoped<IPrescriptionValidationService, PrescriptionValidationService>();

        return services;
    }
}
