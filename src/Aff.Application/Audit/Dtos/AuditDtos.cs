using Aff.Domain.Audit;

namespace Aff.Application.Audit.Dtos;

public record AuditLogResponse(
    Guid Id,
    string EntityType,
    Guid EntityId,
    string Action,
    string Actor,
    string? OldStatus,
    string? NewStatus,
    string? Metadata,
    DateTime Timestamp);

public static class AuditMapper
{
    public static AuditLogResponse ToResponse(AuditLog log) => new(
        log.Id, log.EntityType, log.EntityId, log.Action, log.Actor,
        log.OldStatus, log.NewStatus, log.Metadata, log.Timestamp);
}
