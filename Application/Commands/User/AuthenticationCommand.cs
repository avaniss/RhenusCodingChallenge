using Application.Exceptions;
using Application.Interfaces;
using FluentValidation;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Application.Auth.Commands;
public record AuthenticationCommand : IRequest<AuthenticationCommandResponse>
{
    public string Username { get; init; } = default!;
    public string Password { get; init; } = default!;
}

public record AuthenticationCommandResponse
{
    public string Token { get; init; } = default!;
    public DateTime Expiry { get; init; } = default!;
}

public class AuthenticationCommandValidator : AbstractValidator<AuthenticationCommand>
{
    public AuthenticationCommandValidator()
    {
        _ = RuleFor(v => v)
          .Custom((credentials, context) =>
          {
              if (string.IsNullOrEmpty(credentials.Username) || string.IsNullOrEmpty(credentials.Password))
              {
                  context.AddFailure("Username & Password cannot be empty.");
              }
          });
    }
}

public class AuthenticationCommandHandler : IRequestHandler<AuthenticationCommand, AuthenticationCommandResponse>
{
    private readonly ILogger<AuthenticationCommandHandler> _logger;
    private readonly IApplicationDbContext _context;
    private readonly ISecurityTokenGenerator _securityTokenGenerator;
    public AuthenticationCommandHandler(ILogger<AuthenticationCommandHandler> logger, IApplicationDbContext context, ISecurityTokenGenerator securityTokenGenerator)
    {
        _logger = logger;
        _context = context;
        _securityTokenGenerator = securityTokenGenerator;
    }

    public async Task<AuthenticationCommandResponse> Handle(AuthenticationCommand request, CancellationToken cancellationToken)
    {
        if (this._context.Users.Where(user => user.Username == request.Username && user.Password == request.Password).IsNullOrEmpty())
        {
            throw new UnauthorizedException($"User not found, please verify your credentials!");
        }

        IDictionary<string, string> claims = new Dictionary<string, string>()
        {
            { ClaimTypes.NameIdentifier, request.Username }
        };

        _logger.LogDebug("Generating {} for the user '{}'", _securityTokenGenerator.GetType().Name, request.Username);
        (string token, DateTime expiry) = _securityTokenGenerator.GenerateToken(claims);
        return await Task.Run(() => new AuthenticationCommandResponse { Token = token, Expiry = expiry });
    }
}
