using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ClinicAdoNetApi.DTOs;

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
        var appointments = new List<AppointmentListDto>();

        var connectionString =
            _configuration.GetConnectionString("DefaultConnection");

        await using var connection =
            new SqlConnection(connectionString);

        await connection.OpenAsync();

        const string sql = @"
        SELECT
            a.IdAppointment,
            a.AppointmentDate,
            a.Status,
            a.Reason,
            p.FirstName + N' ' + p.LastName AS PatientFullName,
            p.Email AS PatientEmail
        FROM dbo.Appointments a
        JOIN dbo.Patients p ON p.IdPatient = a.IdPatient
        ORDER BY a.AppointmentDate";

        await using var command =
            new SqlCommand(sql, connection);

        await using var reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            appointments.Add(new AppointmentListDto
            {
                IdAppointment = reader.GetInt32(0),
                AppointmentDate = reader.GetDateTime(1),
                Status = reader.GetString(2),
                Reason = reader.GetString(3),
                PatientFullName = reader.GetString(4),
                PatientEmail = reader.GetString(5)
            });
        }

        return Ok(appointments);
    }
}