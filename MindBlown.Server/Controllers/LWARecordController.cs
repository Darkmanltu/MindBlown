using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindBlown.Server.Models;
using MindBlown.Server.Data;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace MindBlow.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LWARecordController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LWARecordController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<LastWrongAnswerRecord>> GetRecord()
        {
            // returns Record or null if doesn't exist
            var record = await _context.Record.FirstOrDefaultAsync();
            if (record == null)
            {
                return NotFound();
            }
            return Ok(record);
        }

        [HttpPost]
        public async Task<ActionResult<LastWrongAnswerRecord>> PostRecord([FromBody] LastWrongAnswerRecord record)
        {
            try
            {
                var result = await DeleteRecord();
            }
            catch (Exception ex)
            {
                // Optionally handle other exceptions
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            _context.Record.Add(record);
            await _context.SaveChangesAsync();

            return Ok(record);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRecord()
        {
            var record = await _context.Record.FirstOrDefaultAsync();
            
            if (record == null)
            {
                // If not found, returns 404 Not Found
                return NotFound();
            }

            // Delete the record
            _context.Record.Remove(record);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Return 204 No Content status code indicating successful deletion
            return NoContent();
        }

    }
}