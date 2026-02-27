using Microsoft.AspNetCore.Mvc;

namespace AnimalShelterAPI.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        public IActionResult BadRequest(string message)
        {
            return BadRequest(new
            {
                status = 400,
                title = message
            });
        }
    }
}
