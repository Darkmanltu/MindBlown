using System;
using System.ComponentModel.DataAnnotations;

namespace MindBlown.Types;
public class User {

    public  Guid userId { get; set; }
 
    public Guid sessionId { get; set; }
    public DateTime lastActive { get; set; }
    public bool isActive { get; set; }
    
}