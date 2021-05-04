using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using System.Security.Authentication;
using System.Security.Claims;
using Teams.Business.Security;

namespace Teams.Business.Tests
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
            string result = _currentUser.Current.Name();

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
            string result = _currentUser.Current.Id();

            //Assert
            Assert.AreEqual(result, id);
        }

        [Test]
        public void GetExeption_HttpContextAccessorThrownAuthenticationError_ThrownAuthenticationError()
        {

            // Arrange
            _httpContextAccessor.Setup(x => x.HttpContext.User.FindFirst(It.IsAny<string>()))
                .Returns(new Claim("name", "id"));
            _httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(false);

            // Act & Assert
            Assert.Throws<AuthenticationException>(() => _currentUser.Current.Id());
        }
    }
}

