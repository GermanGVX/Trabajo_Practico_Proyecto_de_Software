using Application.Interfaces;
using Application.UseCases.Events.Commands;
using Application.UseCases.Users.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Trabajo_Practoco_Proyecto_de_Software.Controllers
{
    /// <summary>
    /// Gestiona el registro, autenticación y consulta de usuarios del sistema.
    /// </summary>
    [Tags("Usuarios")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ICreateUserCommandHandler _CreateUser;
        private readonly IGetUserByIdQueryHandler _GetUserById;
        private readonly ILoginUserCommandHandler _Login;

        public UsersController(ICreateUserCommandHandler createUser, IGetUserByIdQueryHandler getUserById, ILoginUserCommandHandler login)
        {
            _CreateUser = createUser;
            _GetUserById = getUserById;
            _Login = login;
        }

        /// <summary>
        /// Registra un nuevo usuario en la plataforma.
        /// </summary>
        /// <param name="command">Datos requeridos (Nombre, Email, Password, etc.) para crear la cuenta.</param>
        /// <returns>La ruta para consultar el usuario recién creado y su ID.</returns>
        /// <response code="201">Usuario creado exitosamente.</response>
        /// <response code="400">Error de validación (ej. el email ya existe o datos incompletos).</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> CreateUser(CreateUserCommand command)
        {
            var userId = await _CreateUser.CreateUser(command);
            return CreatedAtAction(nameof(GetUserById), new { id = userId }, new { Id = userId });
        }

        /// <summary>
        /// Inicia sesión en el sistema usando credenciales.
        /// </summary>
        /// <param name="command">Email y contraseña del usuario.</param>
        /// <returns>El ID del usuario autenticado.</returns>
        /// <response code="200">Login exitoso.</response>
        /// <response code="401">Email o contraseña incorrectos.</response>
        /// <response code="400">Error en el formato de la solicitud.</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command)
        {
            var userId = await _Login.LoginUser(command);
            if (userId == null)
                return Unauthorized(new { Error = "Email o contraseña incorrectos" });

            return Ok(new { UserId = userId });
        }

        /// <summary>
        /// Obtiene los detalles de un usuario específico según su ID.
        /// </summary>
        /// <param name="id">El ID único del usuario.</param>
        /// <returns>La información pública del usuario.</returns>
        /// <response code="200">Usuario encontrado exitosamente.</response>
        /// <response code="404">No se encontró ningún usuario con el ID proporcionado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
        public async Task<IActionResult> GetUserById(int id)
        {
            var result = await _GetUserById.GetUserById(id);
            return Ok(result);
        }
    }
}