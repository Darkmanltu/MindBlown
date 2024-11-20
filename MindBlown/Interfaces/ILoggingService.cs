using System.Threading.Tasks;
using MindBlown.Types;

namespace  MindBlown.Interfaces
{
    public interface ILoggingService
    {
        Task LogAsync(LogEntry logEntry);
    }
}