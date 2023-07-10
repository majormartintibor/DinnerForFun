using DFF.BlazorServer.Core.Marten;
using Marten;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using static DFF.BlazorServer.Dinner.DinnerDomainService;

namespace DFF.BlazorServer.Dinner;

public partial class ShowDinner
{
    private Guid dinnerId;
    private Dinner? dinner;
    private DinnerDetails? dinnerDetails;
    private DinnerStatusHistory? dinnerStatusHistory;
    private DinnerTagHistory? dinnerTagHistory;

    private List<string> tags = new()
    {
        "Lactose free",
        "Gluten free",
        "Vegetarian",
        "Vegan",
        "Halal",
        "Koser"
    };
    List<string> selectedTags = new();

    private MudSlider<int>? versionSlider;
    private int versionSliderValue;
    private MudChip[]? selected;

    [Parameter] public string Id { get; set; } = string.Empty;

    [Inject] IDocumentSession Session { get; set; }         

    protected override async Task OnInitializedAsync()
    {
        CancellationToken ct = new();

        if (!Guid.TryParse(Id, out dinnerId)) throw new InvalidCastException("Invalid DinnerId");

        dinner = await Session.GetAggregate<Dinner>(dinnerId, ct);
        dinnerDetails = await Session.Query<DinnerDetails>()
            .SingleOrDefaultAsync(d => d.DinnerId == dinner!.Id);
        versionSliderValue = dinner.Version;
        dinnerStatusHistory = await Session.Query<DinnerStatusHistory>()
            .SingleOrDefaultAsync(d => d.DinnerId == dinner!.Id);
        dinnerTagHistory = await Session.Query<DinnerTagHistory>()
            .SingleOrDefaultAsync(d => d.DinnerId == dinner!.Id);
    }        

    private async Task AnnounceAsync()
    {
        CancellationToken ct = new();

        await Session.GetAndUpdate<Dinner>(
            dinnerId, 
            dinner!.Version, 
            state => Handle(dinner, new AnnounceDinner(dinnerId)),
            ct);

        dinner = await Session.GetAggregate<Dinner>(dinnerId, ct);
        dinnerDetails = await Session.Query<DinnerDetails>()
            .SingleOrDefaultAsync(d => d.DinnerId == dinner!.Id, ct);

        await UpdateProjectionsAsync(dinner.Version);
        versionSliderValue = dinner.Version;
    }

    private async Task AcceptAsync()
    {
        CancellationToken ct = new();

        await Session.GetAndUpdate<Dinner>(
            dinnerId, 
            dinner!.Version, 
            state => Handle(dinner, new AcceptDinner(dinnerId)), 
            ct);

        dinner = await Session.GetAggregate<Dinner>(dinnerId, ct);
        dinnerDetails = await Session.Query<DinnerDetails>()
            .SingleOrDefaultAsync(d => d.DinnerId == dinner!.Id, ct);

        await UpdateProjectionsAsync(dinner.Version);
        versionSliderValue = dinner.Version;
    }

    private async Task FinishAsync()
    {
        CancellationToken ct = new();

        await Session.GetAndUpdate<Dinner>(
            dinnerId, 
            dinner!.Version, 
            state => Handle(dinner, new FinishDinner(dinnerId)), 
            ct);

        dinner = await Session.GetAggregate<Dinner>(dinnerId, ct);
        dinnerDetails = await Session.Query<DinnerDetails>()
            .SingleOrDefaultAsync(d => d.DinnerId == dinner!.Id, ct);

        await UpdateProjectionsAsync(dinner.Version);
        versionSliderValue = dinner.Version;
    }

    private async Task RateAsync()
    {
        CancellationToken ct = new();

        await Session.GetAndUpdate<Dinner>(
            dinnerId, 
            dinner!.Version, 
            state => Handle(dinner, new RateDinner(dinnerId)), 
            ct);

        dinner = await Session.GetAggregate<Dinner>(dinnerId, ct);
        dinnerDetails = await Session.Query<DinnerDetails>()
            .SingleOrDefaultAsync(d => d.DinnerId == dinner!.Id, ct);

        await UpdateProjectionsAsync(dinner.Version);
        versionSliderValue = dinner.Version;        
    }

    // /ShowDinner/79a6026c-68a9-485b-8644-009eeae430e2
    private async Task UpdateProjectionsAsync(int version)
    {
        versionSliderValue = version;

        CancellationToken ct = new();

        dinnerStatusHistory = await Session.Events.AggregateStreamAsync<DinnerStatusHistory>(
            dinner!.Id,
            version,
            token: ct);

        //could be optimized, currently is called when no really needed
        dinnerTagHistory = await Session.Events.AggregateStreamAsync<DinnerTagHistory>(
            dinner!.Id,
            version,
            token: ct);

        StateHasChanged();
    }
    
    private async Task UpdateTagsAsync(IEnumerable<string> values)
    {
        selectedTags = values.ToList();

        CancellationToken ct = new();

        await Session.GetAndUpdate<Dinner>(
            dinnerId,
            dinner!.Version,
            state => Handle(dinner, new UpdateTags(dinnerId, values.ToList())),
            ct);

        dinner = await Session.GetAggregate<Dinner>(dinnerId, ct);
        dinnerDetails = await Session.Query<DinnerDetails>()
            .SingleOrDefaultAsync(d => d.DinnerId == dinner!.Id, ct);

        selectedTags = dinner.Tags;

        await UpdateProjectionsAsync(dinner.Version);
        versionSliderValue = dinner.Version;        
    }
}