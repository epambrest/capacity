using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using Teams.Security;
using System.Security.Authentication;
using NUnit.Framework;

namespace Teams.Tests
{
    [TestFixture]
    public class CurrentUserTest
    {
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private ICurrentUser _currentUser;

        [SetUp]
        public void Setup()
        {
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _currentUser = new CurrentUser(_httpContextAccessor.Object);
        }

        [Test]
        public void GetName_HttpContextAccessorReturnsDzmitry_ReturnsDzmitry()
        {

            // Arrange
            string name = "Dzmitry";
            _httpContextAccessor.Setup(x => x.HttpContext.User.Identity.Name).Returns(name);
            _httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);

            //Act
            string result = _currentUser.GetName();

            //Assert
            Assert.AreEqual(result, name);
        }

        [Test]
        public void GetId_HttpContextAccessorReturns1234_Returns1234()
        {

            // Arrange
            string id = "1234";
            _httpContextAccessor.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
                .Returns(new Claim("name", id));
            _httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);

            //Act
            string result = _currentUser.GetId();

            //Assert
            Assert.AreEqual(result, id);
        }

        [Test]
        public void GetExeption_ HttpContextAccessorThrownAuthenticationError_ThrownAuthenticationError()
        {

            // Arrange
            _httpContextAccessor.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
                .Returns(new Claim("name", "id"));
            _httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(false);

            //Assert
            Assert.Throws<AuthenticationException>(() => _currentUser.GetId());
        }
    }
}

