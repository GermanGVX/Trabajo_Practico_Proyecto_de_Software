using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuditLogRepository
    {
        Task LogAsync(string action, string entityType, string entityId, int? userId, string details);
    }
}
