using Application.Exceptions;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static Domain.Entities.Bet;

namespace Application.Commands;
public record PlaceBetCommand : IRequest<PlaceBetCommandResponse>
{
    public string Username { get; init; } = default!;
    public int Number { get; init; } = default!;
    public long Points { get; init; } = default!;
}

public record PlaceBetCommandResponse
{
    public long Balance { get; set; } = default!;

    public String Status { get; set; } = default!;

    public string Points { get; set; } = default!;
}

public class PlaceBetCommandValidator : AbstractValidator<PlaceBetCommand>
{
    public PlaceBetCommandValidator(IConfiguration configuration)
    {
        int minNum = configuration.GetValue<int>("Bet:Number.Min");
        int maxNum = configuration.GetValue<int>("Bet:Number.Max");

        _ = RuleFor(bet => bet.Username)
            .NotNull()
            .NotEmpty();

        _ = RuleFor(bet => bet)
          .Custom((bet, context) =>
          {
              if (bet.Number < minNum || bet.Number > maxNum)
              {
                  context.AddFailure("Number", $"Number must be between {minNum} and {maxNum}.");
              }

              if (bet.Points <= 0)
              {
                  context.AddFailure("Points", $"Points must be greater than 0.");
              }
          });
    }
}

public class PlaceBetCommandHandler : IRequestHandler<PlaceBetCommand, PlaceBetCommandResponse>
{
    private readonly ILogger<PlaceBetCommandHandler> _logger;
    private readonly IApplicationDbContext _context;
    private readonly Random _numGenerator;
    private readonly int _minVal;
    private readonly int _maxVal;
    private readonly int _winMultiplier;

    public PlaceBetCommandHandler(ILogger<PlaceBetCommandHandler> logger, IApplicationDbContext context, IConfiguration configuration, Random numGenerator)
    {
        this._logger = logger;
        this._context = context;
        this._numGenerator = numGenerator;
        this._minVal = configuration.GetValue<int>("Bet:Number.Min");
        this._maxVal = configuration.GetValue<int>("Bet:Number.Max");
        this._winMultiplier = configuration.GetValue<int>("Bet:Win.Multiplier");
    }

    public async Task<PlaceBetCommandResponse> Handle(PlaceBetCommand request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("The user '{Username}' predicted number to be {Number} for {Points} points.", request.Username, request.Number, request.Points);
        User? user = this._context.Users.FirstOrDefault(x => x.Username == request.Username);
        if (user == null)
        {
            throw new NotFoundException($"User with the provided username '{request.Username}' not found!");
        }

        PlaceBetCommandResponse response;
        var bet = new Bet(user.Id, request.Number, request.Points);
        var num = _numGenerator.Next(this._minVal, this._maxVal);
        _logger.LogDebug("The generated number is '{Number}'.", request.Number);

        if (num == request.Number)
        {
            bet.Status = (int)BetStatus.WON;
            long pointsWon = request.Points * this._winMultiplier;
            user.Balance += pointsWon;
            response = new PlaceBetCommandResponse { Balance = user.Balance, Status = BetStatus.WON.ToString(), Points = $"+{pointsWon}" };
        }
        else
        {
            bet.Status = (int)BetStatus.LOST;
            user.Balance -= request.Points;
            response = new PlaceBetCommandResponse { Balance = user.Balance, Status = BetStatus.LOST.ToString(), Points = $"-{request.Points}" };
        }
        await this._context.Bets.AddAsync(bet);
        await this._context.SaveChangesAsync(cancellationToken);
        return await Task.Run(() => response);
    }
}
