using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MindBlown.Server.Models;
using MindBlown.Server.Data;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MindBlow.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserInfoController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string _secretKey = "7yOWiI8xy8NY3jZd34sbuCRIQ6t6Ut/D09Asn3pxYST7844lJCcYOEwOc6vvK456";  // Secure this key

        public UserInfoController(AppDbContext context)
        {
            _context = context;
        }

        // [HttpGet]
        // public async Task<ActionResult<bool>> IsUsernameUnique([FromBody] UserCredentials user)
        // {
        //     // returns Record or null if doesn't exist
        //     var userInDB = await _context.UserWithMnemonicsIDs.FirstOrDefaultAsync(u => u.Username == user.Username);
        //     if (user == null)
        //     {
        //         return true;
        //     }
        //     return false;
        // }

        [HttpPost("signup")]
        public async Task<ActionResult> PostUser([FromBody] UserCredentials user)
        {
            bool isUnique = await IsUsernameUnique(user);

            if (isUnique)
            {
                var newUserRow = new UserMnemonicIDs {
                    Id = Guid.NewGuid(),
                    Username = user.Username,
                    Password = user.Password,
                    MnemonicGuids = new List<Guid>()
                };

                _context.UserWithMnemonicsIDs.Add(newUserRow);
                await _context.SaveChangesAsync();
                
                return Ok();
            }
            
            return BadRequest("Username is already taken.");
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponse>> LoginUser([FromBody] UserCredentials user)
        {
            var userInDb = await _context.UserWithMnemonicsIDs.FirstOrDefaultAsync(u => u.Username == user.Username);
            if (userInDb == null || userInDb.Password != user.Password)
            {
                return BadRequest("Invalid login credentials");
            }

            var token = GenerateJwtToken(userInDb.Username); // Implement token generation logic
            return Ok(new TokenResponse { Token = token });
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUserMnemonicGuids([FromBody] MnemonicUpdateRequest request)
        {
            // Retrieve the user by Username (or Id)
            var user = await _context.UserWithMnemonicsIDs.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
            {
                // If the user does not exist, return NotFound
                return NotFound("User not found.");
            }

            // Check if the list needs to be updated
            if (request.MnemonicToAdd != null)
            {
                // Adding new mnemonic id to list
                user.MnemonicGuids.Add(request.MnemonicToAdd.Id);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Return Ok to indicate success
                return Ok("Mnemonic GUIDs updated successfully.");
            }

            // If mnemonic == null was passed
            return BadRequest("Invalid mnemonic provided.");
        }

        [HttpGet("guids")]
        public async Task<ActionResult<List<Guid>>> GetMnemonicsGuids([FromQuery] string username)
        {
            var userInDB = await _context.UserWithMnemonicsIDs.FirstOrDefaultAsync(u => u.Username == username);
            if(userInDB != null)
            {
                return Ok(userInDB.MnemonicGuids);
            }
            else
            {
                return BadRequest("User not found");
            }
        }

        public async Task<bool> IsUsernameUnique(UserCredentials user)
        {
            // returns Record or null if doesn't exist
            var userInDB = await _context.UserWithMnemonicsIDs.FirstOrDefaultAsync(u => u.Username == user.Username);
            if (userInDB == null)
            {
                return true;
            }
            return false;
        }

        private string GenerateJwtToken(string username)
        {
            // Claims represent the claims that are going to be in the JWT token
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            // Security key and credentials to sign the token
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create the token
            var token = new JwtSecurityToken(
                issuer: "MindBlownWeb",
                audience: "user-service",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: credentials
            );

            // Convert the token to a string and return
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

public class UserCredentials
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

// public class MnemonicUpdateRequest
// {
//     public required string Username { get; set; }
//     public required Mnemonic MnemonicToAdd { get; set; }
// }