using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                action: "Create_User",
                entityType: "USER",
                entityId: user.Id.ToString(),
                userId: null,
                details: $"Usuario Creado: {user.Name} | Email: {user.Email}"
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
