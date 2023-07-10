using Marten.Events.Aggregation;

namespace DFF.BlazorServer.Dinner;

public sealed record DinnerDetails(
        Guid Id,
        Guid DinnerId,
        Guid HostingUserId,
        List<string> Tags,
        DinnerStatus DinnerStatus,
        int Version = 1
    );

public sealed class DinnerDetailsProjection: SingleStreamProjection<DinnerDetails>
{
    public static DinnerDetails Create(DinnerCreated created)
        => new(Guid.NewGuid(), created.DinnerId, created.HostingUserId, new(), DinnerStatus.New);

    public DinnerDetails Apply(TagUpdated tagAdded, DinnerDetails current)
        => current with { Tags = tagAdded.Tags };    

    public DinnerDetails Apply(DinnerAnnounced announced, DinnerDetails current)
        => current with { DinnerStatus = DinnerStatus.Announced };

    public DinnerDetails Apply(DinnerAccepted accepted, DinnerDetails current)
        => current with { DinnerStatus = DinnerStatus.Accepted };

    public DinnerDetails Apply(DinnerFinished finished, DinnerDetails current)
        => current with { DinnerStatus = DinnerStatus.Finished };

    public DinnerDetails Apply(DinnerRated rated, DinnerDetails current)
        => current with { DinnerStatus = DinnerStatus.Rated };
}

public sealed record DinnerStatusHistory(Guid Id, Guid DinnerId, DinnerStatus DinnerStatus, int Version = 1);

public sealed class DinnerStatusHistoryProjection: SingleStreamProjection<DinnerStatusHistory>
{
    public static DinnerStatusHistory Create(DinnerCreated created)
        => new(Guid.NewGuid(), created.DinnerId, DinnerStatus.New);

    public DinnerStatusHistory Apply(DinnerAnnounced announced, DinnerStatusHistory current)
        => current with { DinnerStatus = DinnerStatus.Announced };

    public DinnerStatusHistory Apply(DinnerAccepted accepted, DinnerStatusHistory current)
        => current with { DinnerStatus = DinnerStatus.Accepted };

    public DinnerStatusHistory Apply(DinnerFinished finished, DinnerStatusHistory current)
        => current with { DinnerStatus = DinnerStatus.Finished };

    public DinnerStatusHistory Apply(DinnerRated rated, DinnerStatusHistory current)
        => current with { DinnerStatus = DinnerStatus.Rated };
}

public sealed record DinnerTagHistory(Guid Id, Guid DinnerId, List<string> Tags, int Veresion = 1);

public sealed class DinnerTagHistoryProjection: SingleStreamProjection<DinnerTagHistory>
{
    public static DinnerTagHistory Create(DinnerCreated created)
        => new(Guid.NewGuid(), created.DinnerId, new());

    public DinnerTagHistory Apply(TagUpdated tagUpdated, DinnerTagHistory current)
        => current with { Tags = tagUpdated.Tags };
}