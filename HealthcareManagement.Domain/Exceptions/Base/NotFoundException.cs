﻿namespace HealthcareManagement.Domain.Exceptions.Base;

public abstract class NotFoundException(string message) : Exception(message)
{ }
