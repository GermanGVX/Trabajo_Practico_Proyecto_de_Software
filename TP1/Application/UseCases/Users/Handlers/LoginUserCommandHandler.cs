using System.Security.Cryptography;
using System.Text;
using Application.Interfaces;
using Application.UseCases.Users.Commands;

namespace Application.UseCases.Users.Handlers
{
    public class LoginUserCommandHandler : ILoginUserCommandHandler
    {
        private readonly IUserRepository _userRepository;

        public LoginUserCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<int> LoginUser(LoginUserCommand command)
        {
            var user = await _userRepository.GetByEmailAsync(command.Email);
            if (user == null)
            {
                throw new Exception("Usuario no existe");
            }
            var inputHash = HashPassword(command.Password);

            if (user.PasswordHash != inputHash)
            {
                throw new Exception("Contraseña incorrecta");
            }

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
