using Application.Commands;
using Application.Exceptions;
using ApplicationTests.Utils;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;

namespace ApplicationTests.Commands
{
    [TestClass]
    public class UserRegistrationCommandTest : BaseTest
    {
        private readonly Mock<ILogger<UserRegistrationCommandHandler>> _logger;
        private readonly IConfiguration _configuraiton;
        private readonly UserRegistrationCommandHandler _command;

        public UserRegistrationCommandTest() : base()
        {
            IEnumerable<KeyValuePair<string, string?>> inMemorySettings = new Dictionary<string, string?> {
                            { "User:InitialBalance", _initialBalance.ToString() }
                        };
            _configuraiton = new ConfigurationBuilder()
                .AddInMemoryCollection(initialData: inMemorySettings)
                .Build();

            _logger = new Mock<ILogger<UserRegistrationCommandHandler>>();
            _command = new UserRegistrationCommandHandler(_logger.Object, _dbContext.Object, _configuraiton);
        }

        [TestMethod]
        public async Task testRegister()
        {
            // Arrange
            var name = "testName";
            var username = "testUsername";
            var password = "testPassword";
            _ = _dbContext.Setup(x => x.Users).ReturnsDbSet(new List<User>(), _userSet);
            UserRegistrationCommand req = new UserRegistrationCommand { Name = name, Username = username, Password = password };

            // Act
            var user = await _command.Handle(req, _cancellationToken);

            // Assert
            _userSet.Verify(s => s.AddAsync(It.IsAny<User>(), _cancellationToken), Times.Once);
            _dbContext.Verify(d => d.SaveChangesAsync(_cancellationToken), Times.Once);
            Assert.IsNotNull(user);
            Assert.IsTrue(user.Id >= 0);
            Assert.AreEqual(name, user.Name);
            Assert.AreEqual(username, user.Username);
            Assert.AreEqual(password, user.Password);
            Assert.AreEqual(_initialBalance, user.Balance);
        }

        [TestMethod]
        public async Task testRegisterUsernameConflict()
        {
            // Arrange
            var user = SetupUser();
            UserRegistrationCommand req = new UserRegistrationCommand { Name = user.Name, Username = user.Username, Password = user.Password };

            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<ConflictException>(() => _command.Handle(req, _cancellationToken));
            Assert.IsTrue(ex.Message.Contains("taken"));
        }
    }
}
