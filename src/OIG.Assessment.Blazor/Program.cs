using OIG.Assessment.Blazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddScoped<CurrentUserState>();

var apiBase = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:65438";
var apiBaseUri = new Uri(apiBase);

builder.Services.AddHttpClient<OrganizationApiClient>(client => client.BaseAddress = apiBaseUri);
builder.Services.AddHttpClient<UsersApiClient>(client => client.BaseAddress = apiBaseUri);
builder.Services.AddHttpClient<RolesApiClient>(client => client.BaseAddress = apiBaseUri);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
