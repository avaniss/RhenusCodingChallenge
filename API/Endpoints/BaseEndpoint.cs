using FastEndpoints;
using MediatR;

namespace API.Endpoints;

/// <summary>
/// Base endpoint class with request and response types.
/// </summary>
public abstract class BaseEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>
    where TRequest : notnull, new()
    where TResponse : notnull, new()
{
    /// <summary>
    /// Mediator instance.
    /// </summary>
    protected ISender Mediator { get; init; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="mediator">Injected mediator instance.</param>
    protected BaseEndpoint(ISender mediator)
    {
        Mediator = mediator;
    }

    /// <summary>
    /// Can be used to provide base functionality for all endpoints.
    /// </summary>
    public override void Configure()
    {
    }
}

/// <summary>
/// Base endpoint class with request type.
/// </summary>
public abstract class BaseEndpoint<TRequest> : Endpoint<TRequest> where TRequest : notnull, new()
{
    /// <summary>
    /// Mediator instance.
    /// </summary>
    protected ISender Mediator { get; init; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="mediator">Injected mediator instance.</param>
    protected BaseEndpoint(ISender mediator)
    {
        Mediator = mediator;
    }

    /// <summary>
    /// Can be used to provide base functionality for all endpoints.
    /// </summary>
    public override void Configure()
    {
    }
}
