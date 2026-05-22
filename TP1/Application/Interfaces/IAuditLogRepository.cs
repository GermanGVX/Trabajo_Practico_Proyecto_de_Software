namespace Application.Interfaces
{
    public interface IAuditLogRepository
    {
        Task LogAsync(string action, string entityType, string entityId, int? userId, string details);
        Task SaveChangesAsync();
    }
}
