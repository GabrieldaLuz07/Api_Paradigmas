using System;

namespace ApiTF.Services.Exceptions
{
    public class InvalidEntityExceptions : Exception
    {
        public InvalidEntityExceptions(string message) : base(message) { }
    }
}
