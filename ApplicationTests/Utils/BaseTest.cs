using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;

namespace ApplicationTests.Utils
{
    public class BaseTest
    {
        public readonly Mock<IApplicationDbContext> _dbContext;
        public readonly Mock<DbSet<User>> _userSet;
        public readonly Mock<DbSet<Bet>> _betSet;
        public readonly CancellationToken _cancellationToken;
        public static readonly long _initialBalance = 10000;

        public BaseTest()
        {
            _cancellationToken = default;
            _dbContext = new Mock<IApplicationDbContext>();
            _userSet = new Mock<DbSet<User>>();
            _betSet = new Mock<DbSet<Bet>>();
        }

        protected User SetupUser()
        {
            var name = "testName";
            var username = "testUsername";
            var password = "testPassword123!";
            var user = new User(name, username, password, _initialBalance);
            return SetupUser(user);
        }

        protected User SetupUser(User user)
        {
            _ = _dbContext.Setup(x => x.Users).ReturnsDbSet(new List<User> { user }, _userSet);
            return user;
        }
    }
}
