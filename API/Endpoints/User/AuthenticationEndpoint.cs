using Application.Auth.Commands;
using FastEndpoints;
using MediatR;

namespace API.Endpoints
{
    public class AuthenticationSummary : Summary<AuthenticationEndpoint>
    {
        public AuthenticationSummary()
        {
            Summary = "Authenticates user";
            Description =
                "This endpoint will authenticate the user credentials and returns a token valid for certain period.";
            Response(400, "Bad Request");
            Response(401, "Unauthorized");
            Response(500, "Internal server error.");
        }
    }

    public class AuthenticationEndpoint : BaseEndpoint<AuthenticationCommand>
    {
        public AuthenticationEndpoint(ISender mediator) : base(mediator)
        {
        }

        public override void Configure()
        {
            base.Configure();
            AllowAnonymous();
            Post("authenticate");
            Description(
                d => d.WithTags("Auth")
            );
            Summary(new AuthenticationSummary());
        }

        public override async Task HandleAsync(AuthenticationCommand req, CancellationToken ct)
        {
            var result = await Mediator.Send(
            new AuthenticationCommand
            {
                Username = req.Username,
                Password = req.Password
            },
            ct
        );
            await SendOkAsync(result);
        }
    }
}