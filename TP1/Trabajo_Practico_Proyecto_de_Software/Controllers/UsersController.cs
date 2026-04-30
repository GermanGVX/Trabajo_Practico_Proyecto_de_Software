using System.Threading.Tasks;
using Application.Interfaces;
using Application.UseCases.Events.Commands;
using Application.UseCases.Users.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Trabajo_Practoco_Proyecto_de_Software.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ICreateUserCommandHandler _CreateUser;
        private readonly IGetUserByIdQueryHandler _GetUserById;
        private readonly ILoginUserCommandHandler _Login;

        public UsersController(ICreateUserCommandHandler createUser, IGetUserByIdQueryHandler getUserById, ILoginUserCommandHandler login )
        {
            _CreateUser = createUser;
            _GetUserById = getUserById;
            _Login = login;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserCommand command)
        {
            var userId = await _CreateUser.CreateUser(command);
            return CreatedAtAction(nameof(GetUserById), new { id = userId }, new { Id = userId });
        }
        [HttpPost("login")] // Login
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            var userId = await _Login.LoginUser(command);
            if (userId == null)
                return Unauthorized(new { Error = "Email o contraseña incorrectos" });

            return Ok(new { UserId = userId });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _GetUserById.GetUserById(id);
            return Ok(result);
        }
    }
}
