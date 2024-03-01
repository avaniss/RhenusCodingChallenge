using Application.Auth.Commands;
using Application.Exceptions;
using Application.Interfaces;
using ApplicationTests.Utils;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApplicationTests.Commands
{
    [TestClass]
    public class AuthenticationCommandTest : BaseTest
    {
        private readonly Mock<ILogger<AuthenticationCommandHandler>> _logger;
        private readonly Mock<ISecurityTokenGenerator> _tokenGenerator;
        private readonly AuthenticationCommandHandler _command;

        public AuthenticationCommandTest() : base()
        {
            _logger = new Mock<ILogger<AuthenticationCommandHandler>>();
            _tokenGenerator = new Mock<ISecurityTokenGenerator>();
            _command = new AuthenticationCommandHandler(_logger.Object, _dbContext.Object, _tokenGenerator.Object);
        }

        [TestMethod]
        public async Task testAuthenticate()
        {
            // Arrange
            var token = "RandomToken";
            var expiry = DateTime.Now;
            var user = SetupUser();
            _ = _tokenGenerator.Setup(x => x.GenerateToken(It.IsAny<IDictionary<string, string>>())).Returns((token, expiry));
            AuthenticationCommand req = new AuthenticationCommand { Username = user.Username, Password = user.Password };

            // Act
            var authenticationResponse = await _command.Handle(req, _cancellationToken);

            // Assert
            Assert.IsNotNull(authenticationResponse);
            Assert.AreEqual(token, authenticationResponse.Token);
            Assert.AreEqual(expiry, authenticationResponse.Expiry);
        }

        [TestMethod]
        public async Task testAuthenticateInvalidCredentials()
        {
            // Arrange
            var user = SetupUser();
            AuthenticationCommand req = new AuthenticationCommand { Username = "foo", Password = "bar" };

            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<UnauthorizedException>(() => _command.Handle(req, _cancellationToken));
            Assert.IsTrue(ex.Message.Contains("please verify"));
        }
    }
}
