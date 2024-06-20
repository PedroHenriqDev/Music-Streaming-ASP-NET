namespace DomainLayer.Exceptions;

public class CloudException : Exception
{
    public CloudException(string message) : base(message)
    {
    }
}
