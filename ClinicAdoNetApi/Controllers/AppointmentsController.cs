using Microsoft.AspNetCore.Mvc;

namespace ClinicAdoNetApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AppointmentsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var connectionString =
            _configuration.GetConnectionString("DefaultConnection");

        return Ok(connectionString);
    }
}