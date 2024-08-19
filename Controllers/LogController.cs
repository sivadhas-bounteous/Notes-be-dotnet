using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration; // Add this
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
using Notes.DTO;
using Microsoft.AspNetCore.Authorization;

namespace Notes.Controllers
{
    [ApiController]
    [Authorize(Policy = "AdminPolicy")]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly string _connectionString;

        public LogsController(IConfiguration configuration) // Inject IConfiguration
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public async Task<IActionResult> GetLogs()
        {
            var logs = new List<LogEntry>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand("SELECT * FROM logs ORDER BY Timestamp DESC", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var logEntry = new LogEntry
                            {
                                Message = reader.GetString(0),
                                MessageTemplate = reader.GetString(1),
                                LogLevel = reader.GetInt32(2), // Adjust based on actual type in your schema
                                Timestamp = reader.GetDateTime(3),
                                Exception = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Properties = reader.IsDBNull(5) ? null : reader.GetString(5)
                            };

                            logs.Add(logEntry);
                        }
                    }
                }
            }

            return Ok(logs);
        }
    }
}
