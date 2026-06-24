using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

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
    public async Task<IActionResult> Get()
    {
        var connectionString =
            _configuration.GetConnectionString("DefaultConnection");

        try
        {
            await using var connection =
                new SqlConnection(connectionString);

            await connection.OpenAsync();

            return Ok("Database connection successful");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}