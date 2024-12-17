using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using MindBlown.Server.Models;
using MindBlown.Server.Data;
using Microsoft.VisualBasic;

namespace MindBlow.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MnemonicsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MnemonicsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/mnemonics
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Mnemonic>>> GetMnemonics()
        {
            var mnemonics = await _context.Mnemonics.ToListAsync();
            return Ok(mnemonics);
        }

        // If not needed, in future delete below Tasks

        // GET: api/mnemonics/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Mnemonic>> GetMnemonic(Guid id)
        {
            var mnemonic = await _context.Mnemonics.FindAsync(id);

            if (mnemonic == null)
            {
                return NotFound();
            }

            return Ok(mnemonic); // Explicitly return Ok(mnemonic)
        }

        // POST: api/mnemonics
        [HttpPost]
        public async Task<ActionResult<Mnemonic>> PostMnemonic([FromBody] Mnemonic mnemonic)
        {
            _context.Mnemonics.Add(mnemonic);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMnemonic), new { id = mnemonic.Id }, mnemonic);
        }

        // POST: api/mnemonics/bulk
        [HttpPost("bulk")]
        public async Task<ActionResult> PostMnemonics(List<Mnemonic> mnemonics)
        {
            if (mnemonics == null || !mnemonics.Any())
            {
                return BadRequest("No mnemonics provided.");
            }

            // Get existing mnemonics from the database
            var existingMnemonics = await _context.Mnemonics.ToListAsync();

            // Filter out duplicates
            var uniqueMnemonics = mnemonics
                .Where(mnemonic => !existingMnemonics.Any(e =>
                    e.HelperText == mnemonic.HelperText && e.MnemonicText == mnemonic.MnemonicText))
                .ToList();


            if (uniqueMnemonics.Any())
            {
                await _context.Mnemonics.AddRangeAsync(uniqueMnemonics);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetMnemonics), uniqueMnemonics);
        }

        // PUT: api/mnemonics/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMnemonic(Guid id, Mnemonic mnemonic)
        {
            if (id != mnemonic.Id)
            {
                return BadRequest();
            }

            _context.Entry(mnemonic).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MnemonicExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/mnemonics/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMnemonic(Guid id)
        {
            var mnemonic = await _context.Mnemonics.FindAsync(id);
            if (mnemonic == null)
            {
                return NotFound();
            }

            _context.Mnemonics.Remove(mnemonic);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Only for testing purposes, delete later
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("API is reachable");
        }


        public bool MnemonicExists(Guid id)
        {
            return _context.Mnemonics.Any(e => e.Id == id);
        }
    }
}
