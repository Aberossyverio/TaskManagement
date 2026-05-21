using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TaskApi.Data;
using TaskApi.Features.Auth;
using TaskApi.Features.Auth.Domain;
using TaskApi.Core.JsonConverter;
using FastEndpoints.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentityCore<User>()
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UnitOfWork>();

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

builder.Services
    .AddAuthenticationJwtBearer(s => s.SigningKey = builder.Configuration["Jwt:SecretKey"])
    .AddFastEndpoints()
    .SwaggerDocument()
    .ConfigureHttpJsonOptions(o =>
    {
        o.SerializerOptions.Converters.Add(new NumberJsonConverter());
        o.SerializerOptions.Converters.Add(new TrimmingStringJsonConverter());
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod()
              .WithExposedHeaders("Content-Disposition");
    });
});

var app = builder.Build();

if (app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

if (app.Environment.IsDevelopment())
{
    app.UseCors();
    app.UseSwaggerGen();
}
else
{
    var allowedOrigins = app.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
    app.UseCors(policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .WithExposedHeaders("Content-Disposition"));
}

app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();

// Run migrations
{
    await using var scope = app.Services.CreateAsyncScope();
    await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
    await DbSeeder.SeedAsync(scope.ServiceProvider);
}

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
    .AllowAnonymous();

app.Run();
