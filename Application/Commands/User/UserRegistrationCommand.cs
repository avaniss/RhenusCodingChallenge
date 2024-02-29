using Application.Exceptions;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Application.Commands
{
    public record UserRegistrationCommand : IRequest<User>
    {
        public string Name { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    public class UserRegistrationCommandValidator : AbstractValidator<UserRegistrationCommand>
    {
        private static readonly Regex passwordMatcher = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
        public UserRegistrationCommandValidator()
        {
            _ = RuleFor(c => c.Name)
                .NotNull()
                .NotEmpty();

            _ = RuleFor(c => c.Username)
                .NotNull()
                .NotEmpty();

            _ = RuleFor(c => c.Password)
                .NotNull()
                .NotEmpty()
                .Custom((password, context) =>
                {
                    if (!passwordMatcher.IsMatch(password))
                    {
                        context.AddFailure("Password", "Password must have minimum 8 characters consisting " +
                                           "at least one uppercase letter, one lowercase letter, " +
                                           "one digit and one special character.");
                    }
                });
        }
    }

    public class UserRegistrationCommandHandler : IRequestHandler<UserRegistrationCommand, User>
    {
        private readonly ILogger<UserRegistrationCommandHandler> _logger;
        private readonly IApplicationDbContext _dbContext;
        private readonly long _initialBalance;

        public UserRegistrationCommandHandler(ILogger<UserRegistrationCommandHandler> logger, IApplicationDbContext dbContext, IConfiguration configuration)
        {
            _logger = logger;
            _dbContext = dbContext;
            _initialBalance = configuration.GetValue<long>("User:InitialBalance");
        }

        public async Task<User> Handle(UserRegistrationCommand request, CancellationToken cancellationToken)
        {
            User? user = _dbContext.Users.Where(user => user.Username.Equals(request.Username)).FirstOrDefault();
            if (user != null)
            {
                throw new ConflictException("Username is already taken, please use a different username.");
            }

            User newUser = new User(request.Name, request.Username, request.Password, _initialBalance);

            await _dbContext.Users.AddAsync(newUser);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("User with the username={Username} has been created successfully.", request.Username);
            return await Task.Run(() => newUser);
        }
    }
}
