namespace Common.Infrastructure.Exceptions;


public abstract class InfrastructureException : Exception
{
    protected InfrastructureException(string message) : base(message) { }
    protected InfrastructureException(string message, Exception innerException) : base(message, innerException) { }
}

public class CachingException : InfrastructureException
{
    public CachingException(string message) : base(message) { }
    public CachingException(string message, Exception innerException) : base(message, innerException) { }
}

public class HttpClientException : InfrastructureException
{
    public int? StatusCode { get; }
    
    public HttpClientException(string message) : base(message) { }
    public HttpClientException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
    public HttpClientException(string message, Exception innerException) : base(message, innerException) { }
}

public class MessagingException : InfrastructureException
{
    public MessagingException(string message) : base(message) { }
    public MessagingException(string message, Exception innerException) : base(message, innerException) { }
}

public class DatabaseConnectionException : InfrastructureException
{
    public DatabaseConnectionException(string message) : base(message) { }
    public DatabaseConnectionException(string message, Exception innerException) : base(message, innerException) { }
}

public class ExternalServiceTimeoutException : InfrastructureException
{
    public string ServiceName { get; }
    
    public ExternalServiceTimeoutException(string serviceName, string message) : base(message)
    {
        ServiceName = serviceName;
    }
    public ExternalServiceTimeoutException(string serviceName, string message, Exception innerException) 
        : base(message, innerException)
    {
        ServiceName = serviceName;
    }
}
