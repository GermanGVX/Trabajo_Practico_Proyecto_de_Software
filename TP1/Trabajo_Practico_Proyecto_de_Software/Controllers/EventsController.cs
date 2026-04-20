using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Trabajo_Practoco_Proyecto_de_Software.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IServicesGetAll _services;

        public EventsController(IServicesGetAll services)
        {
            _services = services;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result= _services.GetAll();
            return new JsonResult(result);
        }
    }
}
