using MindBlown.Types;
using Xunit;

namespace MindBlown.Types.Tests
{
    public class AccRequestTests
    {
        [Fact]
        public void AccRequest_CanBeInitializedWithProperties()
        {
            
            var username = "testUser";
            var password = "testPassword";

            
            var accRequest = new AccRequest
            {
                Username = username,
                Password = password
            };

            
            Assert.Equal(username, accRequest.Username);
            Assert.Equal(password, accRequest.Password);
        }

        [Fact]
        public void AccRequest_CanHandleNullValues()
        {
            
            var accRequest = new AccRequest
            {
                Username = null,
                Password = null
            };

            
            Assert.Null(accRequest.Username);
            Assert.Null(accRequest.Password);
        }

        [Fact]
        public void AccRequest_ShouldSetAndGetUsernameCorrectly()
        {
            
            var username = "testUser";

            
            var accRequest = new AccRequest();
            accRequest.Username = username;

            
            Assert.Equal(username, accRequest.Username);
        }

        [Fact]
        public void AccRequest_ShouldSetAndGetPasswordCorrectly()
        {
            
            var password = "testPassword";

            
            var accRequest = new AccRequest();
            accRequest.Password = password;

            
            Assert.Equal(password, accRequest.Password);
        }
    }
}