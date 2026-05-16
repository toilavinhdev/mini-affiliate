using Aff.Domain.Common;

namespace Aff.Domain.Audit;

public class AuditLog : Entity
{
    public string EntityType { get; private set; } = default!;
    public Guid EntityId { get; private set; }
    public string Action { get; private set; } = default!;
    public string Actor { get; private set; } = default!;
    public string? OldStatus { get; private set; }
    public string? NewStatus { get; private set; }
    public string? Metadata { get; private set; }
    public DateTime Timestamp { get; private set; }

    private AuditLog() { }

    public static AuditLog Create(
        string entityType, Guid entityId, string action,
        string? oldStatus = null, string? newStatus = null,
        string? metadata = null, string actor = "system")
    {
        return new AuditLog
        {
            Id = Guid.NewGuid(),
            EntityType = entityType,
            EntityId = entityId,
            Action = action,
            Actor = actor,
            OldStatus = oldStatus,
            NewStatus = newStatus,
            Metadata = metadata,
            Timestamp = DateTime.UtcNow
        };
    }
}
