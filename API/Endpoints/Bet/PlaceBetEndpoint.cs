using Application.Commands;
using FastEndpoints;
using MediatR;
using System.Security.Claims;

namespace API.Endpoints
{
    public record PlaceBetRequest
    {
        public int Number { get; set; } = default!;

        public long Points { get; set; } = default!;
    }

    public class PlaceBetEndpointSummary : Summary<PlaceBetEndpoint>
    {
        public PlaceBetEndpointSummary()
        {
            Summary = "Places a bet of specified points on a number.";
            Description = "Places a bet of specified points on a number.";
            Response(400, "Bad Request");
            Response(401, "Unauthorized");
            Response(500, "Internal server error");
        }
    }

    public class PlaceBetEndpoint : BaseEndpoint<PlaceBetRequest>
    {
        public PlaceBetEndpoint(ISender mediator) : base(mediator)
        {
        }

        public override void Configure()
        {
            Post("bet");
            Description(
                d => d.WithTags("Bet")
            );
            Summary(new PlaceBetEndpointSummary());
        }

        public async override Task HandleAsync(PlaceBetRequest req, CancellationToken ct)
        {
            var result = await Mediator.Send(
            new PlaceBetCommand
            {
                Username = User.Claims.First(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value,
                Number = req.Number,
                Points = req.Points
            },
            ct
        );
            await SendOkAsync(result);
        }
    }
}
