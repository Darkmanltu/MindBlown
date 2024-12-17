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
        public async Task<ActionResult<LastWrongAnswerRecord>> GetRecord([FromQuery] Guid id)
        {
            // returns Record or null if doesn't exist
            var record = await _context.Record.FirstOrDefaultAsync(r => r.Id == id);
            if (record == null)
            {
                return new LastWrongAnswerRecord{Id = Guid.Empty, helperText = null, mnemonicText = null, wrongTextMnemonic = null};
            }
            return Ok(record);
        }

        [HttpPost]
        public async Task<ActionResult<LastWrongAnswerRecord>> PostRecord([FromBody] IdLWARecordRequest request)
        {
            try
            {
                var result = await DeleteRecord(request.IdToChange);
            }
            catch (Exception ex)
            {
                // Optionally handle other exceptions
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            _context.Record.Add(request.RecordToSet);
            await _context.SaveChangesAsync();

            return Ok(request.RecordToSet);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRecord(Guid id)
        {
            var record = await _context.Record.FirstOrDefaultAsync(r => r.Id == id);
            
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
    public class IdLWARecordRequest
    {
        public required Guid IdToChange { get; set; }
        public required LastWrongAnswerRecord RecordToSet { get; set; }
    }
}