using MindBlown.Server.Singleton;
using Xunit;

public class ActiveUserServiceTests
{
    [Fact]
    public void AddUser_ShouldIncrementUserCount()
    {
        
        var service = new ActiveUserService();

        
        service.AddUser();

        
        Assert.Equal(1, service.UserCount);
    }

    [Fact]
    public void RemoveUser_ShouldDecrementUserCount()
    {
        
        var service = new ActiveUserService();
        service.AddUser(); 

        
        service.RemoveUser();

        
        Assert.Equal(0, service.UserCount);
    }

    [Fact]
    public void UserCount_ShouldNotGoNegative()
    {

        var service = new ActiveUserService();

        
        service.RemoveUser(); 

        
        Assert.True(service.UserCount >= 0, "User count should not be negative.");
    }

    [Fact]
    public void AddUser_MultipleTimes_ShouldCorrectlyIncrementUserCount()
    {
        
        var service = new ActiveUserService();
        int addCount = 5;

        
        for (int i = 0; i < addCount; i++)
        {
            service.AddUser();
        }

        
        Assert.Equal(addCount, service.UserCount);
    }

    [Fact]
    public void AddAndRemoveUser_ShouldMaintainCorrectUserCount()
    {
        
        var service = new ActiveUserService();
        int addCount = 5;
        int removeCount = 3;

        
        for (int i = 0; i < addCount; i++)
        {
            service.AddUser();
        }

        for (int i = 0; i < removeCount; i++)
        {
            service.RemoveUser();
        }

        
        Assert.Equal(addCount - removeCount, service.UserCount);
    }
}