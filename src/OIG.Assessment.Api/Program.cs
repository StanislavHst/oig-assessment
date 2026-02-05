using OIG.Assessment.Api;
using OIG.Assessment.Api.Configuration;
using OIG.Assessment.Api.Extensions;
using OIG.Assessment.Api.Permissions;
using OIG.Assessment.Api.Services;
using OIG.Assessment.Application;
using OIG.Assessment.Application.Services;
using OIG.Assessment.Domain;
using OIG.Assessment.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserContext, CurrentUserContext>();
builder.Services.AddControllers();

builder.Services.AddApplication();
builder.Services.AddDomain();

builder.Services.AddInfrastructure(builder.Configuration.GetPostgreSQLConnectionString());

builder.Services.AddSwaggerWithDemoUser();

var app = builder.Build();

await app.MigrateAndSeedAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseMiddleware<PermissionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
