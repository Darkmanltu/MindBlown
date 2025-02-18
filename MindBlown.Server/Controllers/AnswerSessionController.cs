using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindBlown.Server.Data;
using MindBlown.Server.Models;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.Threading.Tasks;


namespace MindBlown.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerSessionController : ControllerBase
    {
        private readonly AppDbContext _context;

        // Constructor to inject AppDbContext into the controller
        public AnswerSessionController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/AnswerSession/add
        [HttpPost ("add")]
        public async Task<ActionResult<AnswerSession>> CreateAnswerSession([FromBody] AnswerSession answerSession)
        {
            
            // Ensure that required properties are set
            if (answerSession == null)
            {
                return BadRequest("Answer session data is required.");
            }

            
            answerSession.LastAnswerTime = DateTime.UtcNow + TimeSpan.FromMinutes(120); // Set the current time for the last answer

            // Add the new AnswerSession to the DbSet
            _context.AnswerSessions.Add(answerSession);
             await _context.SaveChangesAsync();
            
            // Return the created AnswerSession with a 201 status code (Created)
            return CreatedAtAction(nameof(GetAnswerSession), new { id = answerSession.AnswerSessionId }, answerSession);
        }

       [HttpGet("list")]
        // GET: api/AnswerSession/list?user={userId}
        public async Task<ActionResult<IEnumerable<AnswerSession>>> GetAnswerSessionList(string user)
        {
            if (string.IsNullOrEmpty(user))
            {
                return BadRequest("User identifier is required.");
            }

            var answerSession = await _context.AnswerSessions.FirstOrDefaultAsync(s => s.UserName == user);
            if (answerSession == null)
            {
                return new List<AnswerSession>();
            }

            var answerSessions = await _context.AnswerSessions
                .Where(s => s.UserName == user) // Assuming AnswerSession has UserId property
                .ToListAsync();

            if (answerSessions == null || !answerSessions.Any())
            {
                return NotFound("No sessions found for this user.");
            }

            return Ok(answerSessions); // Return list of AnswerSessions
        }
        [HttpGet ("{id}")]
        public async Task<ActionResult<AnswerSession>> GetAnswerSession(Guid id)
        {
            var answerSession = await _context.AnswerSessions.FindAsync(id);

            if (answerSession == null)
            {
                return NotFound();
            }

            return Ok(answerSession);
        }
        [HttpPost("addAnsweredMnemonic")]
        public async Task<IActionResult> AddAnsweredMnemonic([FromBody] AnsweredMnemonic answeredMnemonic)
        {
            var answerSessionForMnemonic = await _context.AnswerSessions.FindAsync(answeredMnemonic.AnswerSessionId);
            var m = new AnsweredMnemonic{
                AnsweredMnemonicId = Guid.NewGuid(),
                AnswerSessionId = answeredMnemonic.AnswerSessionId,
                MnemonicId = answeredMnemonic.MnemonicId,
                IsCorrect = answeredMnemonic.IsCorrect,
                AnswerSession = answerSessionForMnemonic!
            };
            if (answeredMnemonic == null)
            {
                
            }

            // Log the incoming request data
          
            try
            {
                    // Ensure the AnswerSession exists
                if (answeredMnemonic != null && answeredMnemonic.AnswerSessionId != Guid.Empty)
                {
                    var answerSession = await _context.AnswerSessions.FindAsync(answeredMnemonic.AnswerSessionId);
                    if (answerSession == null)
                    {
                        return NotFound();
                    }
                

                _context.AnsweredMnemonics.Add(m);
                await _context.SaveChangesAsync();

                answerSession.LastAnswerTime = DateTime.UtcNow + TimeSpan.FromMinutes(120);
                
                    try
                    {
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateException ex)
                    {
                        return StatusCode(500, $"Internal server error: {ex.Message}");
                    }

                }
            }
            catch (Exception e)
            {
                
                return BadRequest("AnswerSession was not generated yet" + e.Message);
            }
        
            return Ok();
        }

        [HttpPost ("remove")]
        public async Task<IActionResult> RemoveAnswerSession(Guid id)
         {
            var answerSession = await _context.AnswerSessions.FindAsync(id);

            if (answerSession == null)
            {
                return NotFound();
            }

            _context.AnswerSessions.Remove(answerSession);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            return Ok();
        }
    }
}
