// not used...
namespace MindBlown.Server.Singleton{
    public class ActiveUserService 
{
    private int _userCount = 0;

    public int UserCount => _userCount;

    public void AddUser()
    {
        Interlocked.Increment(ref _userCount);
    }

    public void RemoveUser()
    {
        Interlocked.Decrement(ref _userCount);
    }

}
}
