using Aff.Application.Audit.Dtos;
using Aff.Domain.Audit;
using Aff.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Aff.Application.Audit;

public class AuditService(AffDbContext db)
{
    public void Log(string entityType, Guid entityId, string action,
        string? oldStatus = null, string? newStatus = null,
        string? metadata = null, string actor = "system")
    {
        db.AuditLogs.Add(AuditLog.Create(entityType, entityId, action, oldStatus, newStatus, metadata, actor));
    }

    public async Task<List<AuditLogResponse>> GetLogsAsync(string? entityType = null, Guid? entityId = null)
    {
        var query = db.AuditLogs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(entityType))
            query = query.Where(l => l.EntityType == entityType);
        if (entityId.HasValue)
            query = query.Where(l => l.EntityId == entityId.Value);
        var logs = await query.OrderByDescending(l => l.Timestamp).ToListAsync();
        return logs.Select(AuditMapper.ToResponse).ToList();
    }
}
