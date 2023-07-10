using Marten;

namespace DFF.BlazorServer.Core.Marten;

public static class DocumentSessionExtensions
{
    public static async Task Add<T>(
        this IDocumentSession documentSession,
        Guid id, object @event, CancellationToken ct
    ) where T : class
    {
        documentSession.Events.StartStream<T>(id, @event);

        await documentSession.SaveChangesAsync(token: ct);
    }

    public static async Task GetAndUpdate<T>(
        this IDocumentSession documentSession,
        Guid id, 
        int version,
        Func<T, object> handle,
        CancellationToken ct
    ) where T : class
    {
        await documentSession.Events.WriteToAggregate<T>(id, version, stream =>
            stream.AppendOne(handle(stream.Aggregate)), ct);
    }

    public static async Task<T> GetAggregate<T>(
        this IDocumentSession documentSession,
        Guid id,
        CancellationToken ct
        ) where T : class
    {
        return await documentSession.Events.AggregateStreamAsync<T>(id, token: ct)
            ?? throw new InvalidOperationException($"{typeof(T).Name} with id '{id}' was not found");
    }    
}
