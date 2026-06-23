using Microsoft.AspNetCore.Mvc;

namespace ClinicAdoNetApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Appointments endpoint works");
    }
}