using DFF.BlazorServer.Core.Marten;
using JasperFx.Core;
using Marten;
using Microsoft.AspNetCore.Components;
using System.Collections.Concurrent;
using static DFF.BlazorServer.Dinner.DinnerDomainService;

namespace DFF.BlazorServer.Dinner;

public partial class ListDinners
{
    [Inject] NavigationManager Navigation { get; set; }
    [Inject] IDocumentSession Session { get; set; }

    private readonly ConcurrentBag<Dinner> dinners = new();

    protected override async Task OnInitializedAsync()
    {
        //Todo: figure out how to make a list, also rather use projections here.

        var dinnersFromDocumentStore = await Session.Query<Dinner>().ToListAsync();

        dinners.Append(dinnersFromDocumentStore);                            
    }

    public async Task CreateNewDinnerAsync()
    {
        var dinnerId = Guid.NewGuid();
        var ct = new CancellationToken();

        await Session.Add<Dinner>(dinnerId, Handle(new CreateDinner(dinnerId, Guid.Empty)), ct);

        Navigation.NavigateTo($"/ShowDinner/{dinnerId}");
    }

    public void Open(Guid dinnerId)
    {
        Navigation.NavigateTo($"/ShowDinner/{dinnerId}");
    }
}