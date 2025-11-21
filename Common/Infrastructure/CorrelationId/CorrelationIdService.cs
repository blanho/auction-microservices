using Common.Application.Abstractions;

namespace Common.Infrastructure.CorrelationId;

public class CorrelationIdService : ICorrelationIdService
{
    private string? _correlationId;

    public string CorrelationId
    {
        get => _correlationId ?? GenerateCorrelationId();
        set => _correlationId = value;
    }

    public string GenerateCorrelationId()
    {
        _correlationId = Guid.NewGuid().ToString();
        return _correlationId;
    }
}
