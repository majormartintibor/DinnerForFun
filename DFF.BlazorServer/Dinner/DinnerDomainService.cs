namespace DFF.BlazorServer.Dinner;

internal static class DinnerDomainService
{
    public static DinnerCreated Handle(CreateDinner command)
    {
        var (dinnerId, hostingUserId) = command;

        return new DinnerCreated(dinnerId, hostingUserId);
    }

    public static DinnerAnnounced Handle(Dinner current, AnnounceDinner command)
    {
        if (current.DinnerStatus != DinnerStatus.New) 
            throw new InvalidOperationException("Dinner is in invalid state");

        return new DinnerAnnounced(command.DinnerId); 
    }

    public static TagUpdated Handle(Dinner current, UpdateTags command)
    {
        if (current.DinnerStatus >= DinnerStatus.Finished)
            throw new InvalidOperationException("Dinner is in invalid state");

        return new TagUpdated(command.DinnerId, command.Tags); 
    }   

    public static DinnerAccepted Handle(Dinner current, AcceptDinner command)
    {
        if (current.DinnerStatus != DinnerStatus.Announced)
            throw new InvalidOperationException("Dinner is in invalid state");

        return new DinnerAccepted(command.DinnerId);
    }

    public static DinnerFinished Handle(Dinner current, FinishDinner command)
    {
        if (current.DinnerStatus != DinnerStatus.Accepted)
            throw new InvalidOperationException("Dinner is in invalid state");

        return new DinnerFinished(command.DinnerId);
    }

    public static DinnerRated Handle(Dinner current, RateDinner command)
    {
        if (current.DinnerStatus != DinnerStatus.Finished)
            throw new InvalidOperationException("Dinner is in invalid state");

        return new DinnerRated(command.DinnerId);
    }
}

public sealed record CreateDinner(Guid Id, Guid HostingUserId);

public sealed record UpdateTags(Guid DinnerId, List<string> Tags);

public sealed record AnnounceDinner(Guid DinnerId);

public sealed record AcceptDinner(Guid DinnerId);

public sealed record FinishDinner(Guid DinnerId);

public sealed record RateDinner(Guid DinnerId);