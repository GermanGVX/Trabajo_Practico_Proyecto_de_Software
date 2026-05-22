using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Application.Interfaces;
using Application.UseCases.Events.Commands;
using Domain.Entities;

namespace Application.UseCases.Events.Handlers
{
    public class CreateUserCommandHandler : ICreateUserCommandHandler
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuditLogRepository _auditLogRepository;

        public CreateUserCommandHandler(IUserRepository userRepository, IAuditLogRepository auditLogRepository)
        {
            _userRepository = userRepository;
            _auditLogRepository = auditLogRepository;
        }

        public async Task<int> CreateUser(CreateUserCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Password) || command.Password.Length < 6)
                throw new ArgumentException("La contraseña debe tener al menos 6 caracteres.");

            var PasswordHash = HashPassword(command.Password);

            var user = new USER
            {
                Name = command.Name,
                Email = command.Email,
                PasswordHash = PasswordHash
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            await _auditLogRepository.LogAsync(
                action: "CREATE_USER",
                entityType: "USER",
                entityId: user.Id.ToString(),
                userId: null,
                details: JsonSerializer.Serialize(new
                {
                    UserId = user.Id,
                    Username = user.Name,
                    Email = user.Email,
                    CreatedAt = DateTime.UtcNow
                })
                );

            return user.Id;
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
