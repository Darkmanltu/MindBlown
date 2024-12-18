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
        // Ensure _userCount does not go below zero
        int initialCount, computedCount;
        do
        {
            initialCount = _userCount;
            if (initialCount == 0)
            {
                return; // Exit early if count is already zero
            }
            computedCount = initialCount - 1;
        }
        while (initialCount != Interlocked.CompareExchange(ref _userCount, computedCount, initialCount));
    }

}
}
