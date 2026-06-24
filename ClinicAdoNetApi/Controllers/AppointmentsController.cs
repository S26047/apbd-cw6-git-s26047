using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ClinicAdoNetApi.DTOs;
using System.Data;

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
    public async Task<IActionResult> Get(
        [FromQuery] string? status,
        [FromQuery] string? patientLastName)
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
            WHERE (@Status IS NULL OR a.Status = @Status)
              AND (@PatientLastName IS NULL OR p.LastName = @PatientLastName)
            ORDER BY a.AppointmentDate";


        await using var command =
            new SqlCommand(sql, connection);

        command.Parameters.Add("@Status", SqlDbType.NVarChar, 30)
            .Value = (object?)status ?? DBNull.Value;

        command.Parameters.Add("@PatientLastName", SqlDbType.NVarChar, 80)
            .Value = (object?)patientLastName ?? DBNull.Value;

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

    [HttpGet("{idAppointment}")]
    public async Task<IActionResult> GetById(int idAppointment)
    {
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
            a.InternalNotes,
            p.FirstName + N' ' + p.LastName,
            p.Email,
            p.PhoneNumber,
            d.FirstName + N' ' + d.LastName,
            d.LicenseNumber,
            s.Name,
            a.CreatedAt
        FROM dbo.Appointments a
        JOIN dbo.Patients p
            ON p.IdPatient = a.IdPatient
        JOIN dbo.Doctors d
            ON d.IdDoctor = a.IdDoctor
        JOIN dbo.Specializations s
            ON s.IdSpecialization = d.IdSpecialization
        WHERE a.IdAppointment = @IdAppointment";
        
        await using var command =
            new SqlCommand(sql, connection);

        command.Parameters.Add(
            "@IdAppointment",
            SqlDbType.Int).Value = idAppointment;

        await using var reader =
            await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return NotFound(new ErrorResponseDto
            {
                Message = "Appointment not found"
            });
        }
        
        var appointment = new AppointmentDetailsDto
        {
            IdAppointment = reader.GetInt32(0),
            AppointmentDate = reader.GetDateTime(1),
            Status = reader.GetString(2),
            Reason = reader.GetString(3),

            InternalNotes =
                reader.IsDBNull(4)
                    ? null
                    : reader.GetString(4),

            PatientFullName = reader.GetString(5),
            PatientEmail = reader.GetString(6),
            PatientPhoneNumber = reader.GetString(7),

            DoctorFullName = reader.GetString(8),
            LicenseNumber = reader.GetString(9),
            Specialization = reader.GetString(10),

            CreatedAt = reader.GetDateTime(11)
        };

        return Ok(appointment);
    }
    


}