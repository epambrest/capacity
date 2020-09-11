using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using Teams.Security;
using Xunit;
using System.Security.Authentication;

namespace Teams.UnitTests
{
    public class CurrentUserTest
    {
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
        private readonly ICurrentUser _currentUser;
        public CurrentUserTest()
        {
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _currentUser = new CurrentUser(_httpContextAccessor.Object);
        }

        [Fact]
        public void GetName_HttpContextAccessorReturnsDzmitry_ReturnsDzmitry()
        {
            // Arrange
            string name = "Dzmitry";
            _httpContextAccessor.Setup(x => x.HttpContext.User.Identity.Name).Returns(name);
            _httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            //Act
            string result = _currentUser.Name();

            //Assert
            Assert.Equal(result, name);

        }

        [Fact]
        public void GetId_HttpContextAccessorReturns1234_Returns1234()
        {
            // Arrange
            string id = "1234";
            _httpContextAccessor.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
                .Returns(new Claim("name", id));
            _httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);

            //Act
            string result = _currentUser.Id();

            //Assert
            Assert.Equal(result, id);
        }

        
         [Fact]
        public void GetExeption_HttpContextAccessorReturnsAuthenticationError_ReturnsAuthenticationError()
        {
            // Arrange
            string text = "authentication error";
            try
            {
                _httpContextAccessor.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
                    .Returns(new Claim("name", "name"));
                _httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(false);
                //Act
                string result = _currentUser.Id();
            }
            catch(AuthenticationException e)
            {
                //Assert
                Assert.Equal(e.Message, text);
            }

        }
    }
}
