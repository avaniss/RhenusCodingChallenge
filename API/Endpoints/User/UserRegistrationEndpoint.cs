using Application.Commands;
using FastEndpoints;
using MediatR;

namespace API.Endpoints
{
    public class UserRegistrationSummary : Summary<UserRegistrationEndpoint>
    {
        public UserRegistrationSummary()
        {
            Summary = "Registers a user";
            Description = "This endpoint registers a user with the provided details.";
            Response(400, "Bad request.");
            Response(409, "Conflict.");
            Response(500, "Internal server error.");
        }
    }
    public class UserRegistrationEndpoint : BaseEndpoint<UserRegistrationCommand>
    {
        public UserRegistrationEndpoint(ISender mediator) : base(mediator)
        {
        }

        public override void Configure()
        {
            base.Configure();
            AllowAnonymous();
            Post("register");
            Description(
                d => d.WithTags("User")
            );
            Summary(new UserRegistrationSummary());
        }

        public override async Task HandleAsync(UserRegistrationCommand req, CancellationToken ct)
        {
            var result = await Mediator.Send(
            new UserRegistrationCommand
            {
                Name = req.Name,
                Username = req.Username,
                Password = req.Password
            },
            ct
        );
            await SendOkAsync(result);
        }
    }
}
