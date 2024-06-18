using FluentValidation.Results;
using System;
using System.Collections.Generic;

namespace ApiTF.Services.Exceptions
{
    public class DataValidationException : Exception
    {
        public IEnumerable<ValidationFailure> ValidationErrors { get; }

        public DataValidationException(string message, IEnumerable<ValidationFailure> validationErrors) : base(message)
        {
            ValidationErrors = validationErrors;
        }
    }
}
