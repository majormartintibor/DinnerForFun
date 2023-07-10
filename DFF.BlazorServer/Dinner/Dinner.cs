namespace DFF.BlazorServer.Dinner;

public sealed record Dinner(
    Guid Id,
    Guid HostingUserId,
    List<string> Tags,
    //consider removing initialization and move setting of DinnerStatus.New to DinnerCreated
    DinnerStatus DinnerStatus = DinnerStatus.New,
    int Version = 1)
{
    public static Dinner Create(DinnerCreated created)
        => new(created.DinnerId, created.HostingUserId, new());

    public Dinner Apply(TagUpdated tagUpdated)
        => UpdateTags(tagUpdated.Tags);

    public Dinner Apply(DinnerAnnounced announced)
        => this with { DinnerStatus = DinnerStatus.Announced };

    public Dinner Apply(DinnerAccepted accepted)
        => this with { DinnerStatus = DinnerStatus.Accepted };

    public Dinner Apply(DinnerFinished finished)
        => this with { DinnerStatus = DinnerStatus.Finished };

    public Dinner Apply(DinnerRated rated)
        => this with { DinnerStatus = DinnerStatus.Rated };

    public Dinner UpdateTags(List<string> tags)
        => this with { Tags = tags };
}

public sealed record DinnerCreated(Guid DinnerId, Guid HostingUserId);

public sealed record TagUpdated(Guid DinnerId, List<string> Tags);

public sealed record DinnerAnnounced(Guid DinnerId);

public sealed record DinnerAccepted(Guid DinnerId);

public sealed record DinnerFinished(Guid DinnerId);

public sealed record DinnerRated(Guid DinnerId);

public enum DinnerStatus
{
    New = 0,
    Announced = 1,
    Accepted = 2,
    Finished = 3,
    Rated = 4
}