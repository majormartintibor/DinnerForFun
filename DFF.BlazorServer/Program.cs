using DFF.BlazorServer.Dinner;
using Marten;
using Marten.Events.Projections;
using MudBlazor.Services;
using Weasel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

builder.Services.AddMarten(sp =>
{
    var options = new StoreOptions();

    // Establish the connection string to your Marten database
    options.Connection(builder.Configuration.GetConnectionString("Default")!);

    options.UseDefaultSerialization(
            EnumStorage.AsInteger
        );

    options.Projections.Add<DinnerDetailsProjection>(ProjectionLifecycle.Inline);
    options.Projections.Add<DinnerStatusHistoryProjection>(ProjectionLifecycle.Inline);
    options.Projections.Add<DinnerTagHistoryProjection>(ProjectionLifecycle.Inline);

    // If we're running in development mode, let Marten just take care
    // of all necessary schema building and patching behind the scenes
    if (builder.Environment.IsDevelopment())
    {
        options.AutoCreateSchemaObjects = AutoCreate.All;
    }

    return options;
})
.UseLightweightSessions();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
