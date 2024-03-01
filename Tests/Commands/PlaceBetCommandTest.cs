using Application.Commands;
using ApplicationTests.Utils;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using static Domain.Entities.Bet;

namespace ApplicationTests.Commands
{
    [TestClass]
    public class PlaceBetCommandTestTest : BaseTest
    {
        private readonly Mock<ILogger<PlaceBetCommandHandler>> _logger;
        private readonly Mock<Random> _random;
        private readonly IConfiguration _configuraiton;
        private readonly PlaceBetCommandHandler _command;
        private static readonly int _minVal = 0;
        private static readonly int _maxVal = 9;
        private static readonly int _winMultiplier = 9;

        public PlaceBetCommandTestTest() : base()
        {
            IEnumerable<KeyValuePair<string, string?>> inMemorySettings = new Dictionary<string, string?> {
                            { "Bet:Number.Min", _minVal.ToString() },
                            { "Bet:Number.Max", _maxVal.ToString() },
                            { "Bet:Win.Multiplier", _winMultiplier.ToString() }
                        };
            _configuraiton = new ConfigurationBuilder()
                .AddInMemoryCollection(initialData: inMemorySettings)
                .Build();

            _logger = new Mock<ILogger<PlaceBetCommandHandler>>();
            _random = new Mock<Random>();
            _command = new PlaceBetCommandHandler(_logger.Object, _dbContext.Object, _configuraiton, _random.Object);
        }

        [TestMethod]
        public async Task testPlaceBetWin()
        {
            // Arrange
            var betNumber = 1;
            var betPoints = 100;
            var user = SetupUser();
            _ = _dbContext.Setup(x => x.Bets).ReturnsDbSet(new List<Bet>(), _betSet);
            _ = _random.Setup(x => x.Next(_minVal, _maxVal)).Returns(betNumber);
            PlaceBetCommand req = new PlaceBetCommand { Username = user.Username, Number = betNumber, Points = betPoints };

            // Act
            var response = await _command.Handle(req, _cancellationToken);

            // Assert
            var wonPoints = betPoints * 9;
            _betSet.Verify(s => s.AddAsync(It.IsAny<Bet>(), _cancellationToken), Times.Once);
            _dbContext.Verify(d => d.SaveChangesAsync(_cancellationToken), Times.Once);
            Assert.IsNotNull(response);
            Assert.AreEqual(BetStatus.WON.ToString(), response.Status);
            Assert.AreEqual($"+{wonPoints}", response.Points);
            Assert.AreEqual(_initialBalance + wonPoints, response.Balance);
        }

        [TestMethod]
        public async Task testPlaceBetLost()
        {
            // Arrange
            var betNumber = 1;
            var betPoints = 100;
            var user = SetupUser();
            _ = _dbContext.Setup(x => x.Bets).ReturnsDbSet(new List<Bet>(), _betSet);
            _ = _random.Setup(x => x.Next(_minVal, _maxVal)).Returns(3);
            PlaceBetCommand req = new PlaceBetCommand { Username = user.Username, Number = betNumber, Points = betPoints };

            // Act
            var response = await _command.Handle(req, _cancellationToken);

            // Assert
            _betSet.Verify(s => s.AddAsync(It.IsAny<Bet>(), _cancellationToken), Times.Once);
            _dbContext.Verify(d => d.SaveChangesAsync(_cancellationToken), Times.Once);
            Assert.IsNotNull(response);
            Assert.AreEqual(BetStatus.LOST.ToString(), response.Status);
            Assert.AreEqual($"-{betPoints}", response.Points);
            Assert.AreEqual(_initialBalance - betPoints, response.Balance);
        }
    }
}
