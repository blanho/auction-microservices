namespace Common.Application.Abstractions;

public interface ICorrelationIdService
{
    string CorrelationId { get; set; }
    string GenerateCorrelationId();
}
