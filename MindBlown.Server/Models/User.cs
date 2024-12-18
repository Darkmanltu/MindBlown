using System;
using System.ComponentModel.DataAnnotations;

namespace MindBlown.Server.Models;
public class User {
// userId is the same as GUID for convience
    public  Guid UserId { get; set; }
    // sessionID is from cookies
    public Guid SessionId { get; set; }
    public DateTime LastActive { get; set; }
    public bool IsActive { get; set; }
    
}