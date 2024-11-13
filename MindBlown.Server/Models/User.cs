using System;
using System.ComponentModel.DataAnnotations;

namespace MindBlown.Server.Models;
public class User {
// userId is the same as GUID for convience
    public  Guid userId { get; set; }
    // sessionID is from cookies (i think)
    public Guid sessionId { get; set; }
    public DateTime lastActive { get; set; }
    public bool isActive { get; set; }
    
}