using Microsoft.AspNetCore.Identity;
using TaskApi.Features.Auth.Domain;
using TaskApi.Features.TaskModule;

namespace TaskApi.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var dbContext = serviceProvider.GetRequiredService<AppDbContext>();

        if (!await roleManager.RoleExistsAsync("admin"))
        {
            await roleManager.CreateAsync(new IdentityRole<int> { Name = "admin" });
        }

        var adminUser = await userManager.FindByNameAsync("admin");
        if (adminUser == null)
        {
            adminUser = new User
            {
                UserName = "admin",
                Email = "admin@taskapi.com",
                EmailConfirmed = true,
                Fullname = "Administrator"
            };
            await userManager.CreateAsync(adminUser, "Admin123*");
            await userManager.AddToRoleAsync(adminUser, "admin");
        }

        if (!dbContext.Tasks.Any())
        {
            var tasks = new[]
            {
                TaskItem.Create("Implement user authentication", "Add JWT-based authentication to the API", "Todo", adminUser.Id),
                TaskItem.Create("Fix memory leak in service", "Investigate and resolve memory leak in background service", "InProgress", adminUser.Id),
                TaskItem.Create("Write unit tests for API", "Achieve 80% code coverage for all endpoints", "Todo", adminUser.Id),
                TaskItem.Create("Optimize database queries", "Add indexes and optimize slow queries", "InProgress", adminUser.Id),
                TaskItem.Create("Update dependencies", "Update all NuGet packages to latest stable versions", "Todo", adminUser.Id),
                TaskItem.Create("Implement caching layer", "Add Redis caching for frequently accessed data", "Todo", adminUser.Id),
                TaskItem.Create("Code review PR #234", "Review and approve pull request for new feature", "Done", adminUser.Id),
                TaskItem.Create("Fix CORS configuration", "Update CORS policy to allow frontend domain", "Done", adminUser.Id),
                TaskItem.Create("Add logging middleware", "Implement structured logging with Serilog", "InProgress", adminUser.Id),
                TaskItem.Create("Refactor payment service", "Break down monolithic payment service into smaller components", "Todo", adminUser.Id),
                TaskItem.Create("Setup CI/CD pipeline", "Configure GitHub Actions for automated deployment", "InProgress", adminUser.Id),
                TaskItem.Create("Document API endpoints", "Add Swagger documentation for all endpoints", "Todo", adminUser.Id),
                TaskItem.Create("Implement rate limiting", "Add rate limiting to prevent API abuse", "Todo", adminUser.Id),
                TaskItem.Create("Fix null reference exception", "Handle null case in user profile endpoint", "Done", adminUser.Id),
                TaskItem.Create("Add input validation", "Implement FluentValidation for all DTOs", "InProgress", adminUser.Id),
                TaskItem.Create("Migrate to .NET 8", "Upgrade project from .NET 6 to .NET 8", "Todo", adminUser.Id),
                TaskItem.Create("Implement WebSocket support", "Add real-time notifications using SignalR", "Todo", adminUser.Id),
                TaskItem.Create("Fix broken integration tests", "Update integration tests after schema changes", "InProgress", adminUser.Id),
                TaskItem.Create("Add health check endpoints", "Implement health checks for monitoring", "Done", adminUser.Id),
                TaskItem.Create("Optimize Docker image", "Reduce Docker image size using multi-stage builds", "Todo", adminUser.Id),
                TaskItem.Create("Implement pagination", "Add pagination support to list endpoints", "Done", adminUser.Id),
                TaskItem.Create("Fix timezone handling", "Ensure all dates are stored and returned in UTC", "InProgress", adminUser.Id),
                TaskItem.Create("Add error tracking", "Integrate Sentry for error monitoring", "Todo", adminUser.Id),
                TaskItem.Create("Implement soft delete", "Add soft delete functionality for entities", "Todo", adminUser.Id),
                TaskItem.Create("Refactor authentication logic", "Extract auth logic into separate service", "InProgress", adminUser.Id),
                TaskItem.Create("Add API versioning", "Implement versioning strategy for API", "Todo", adminUser.Id),
                TaskItem.Create("Fix SQL injection vulnerability", "Use parameterized queries in raw SQL", "Done", adminUser.Id),
                TaskItem.Create("Implement file upload", "Add endpoint for uploading user avatars", "Todo", adminUser.Id),
                TaskItem.Create("Add request validation", "Validate all incoming requests with middleware", "InProgress", adminUser.Id),
                TaskItem.Create("Setup database migrations", "Configure EF Core migrations for production", "Done", adminUser.Id),
                TaskItem.Create("Implement search functionality", "Add full-text search using Elasticsearch", "Todo", adminUser.Id),
                TaskItem.Create("Fix race condition", "Resolve concurrency issue in order processing", "InProgress", adminUser.Id),
                TaskItem.Create("Add email notifications", "Implement email service using SendGrid", "Todo", adminUser.Id),
                TaskItem.Create("Optimize API response time", "Reduce average response time to under 200ms", "InProgress", adminUser.Id),
                TaskItem.Create("Implement audit logging", "Track all data changes for compliance", "Todo", adminUser.Id),
                TaskItem.Create("Fix deadlock in database", "Resolve deadlock issue in transaction handling", "Done", adminUser.Id),
                TaskItem.Create("Add feature flags", "Implement feature toggle system", "Todo", adminUser.Id),
                TaskItem.Create("Refactor repository pattern", "Simplify repository implementation", "InProgress", adminUser.Id),
                TaskItem.Create("Implement data encryption", "Encrypt sensitive data at rest", "Todo", adminUser.Id),
                TaskItem.Create("Add API documentation", "Create comprehensive API documentation", "InProgress", adminUser.Id),
                TaskItem.Create("Fix memory overflow", "Optimize large file processing to prevent OOM", "Done", adminUser.Id),
                TaskItem.Create("Implement backup strategy", "Setup automated database backups", "Todo", adminUser.Id),
                TaskItem.Create("Add monitoring dashboard", "Create Grafana dashboard for metrics", "Todo", adminUser.Id),
                TaskItem.Create("Fix authentication bug", "Resolve token refresh issue", "Done", adminUser.Id),
                TaskItem.Create("Implement multi-tenancy", "Add support for multiple tenants", "Todo", adminUser.Id),
                TaskItem.Create("Optimize entity queries", "Use projection to reduce data transfer", "InProgress", adminUser.Id),
                TaskItem.Create("Add security headers", "Implement security headers middleware", "Done", adminUser.Id),
                TaskItem.Create("Refactor error handling", "Centralize error handling with global exception filter", "InProgress", adminUser.Id),
                TaskItem.Create("Implement data seeding", "Create seed data for development environment", "Done", adminUser.Id),
                TaskItem.Create("Add performance tests", "Create load tests using k6", "Todo", adminUser.Id)
            };

            await dbContext.Tasks.AddRangeAsync(tasks);
            await dbContext.SaveChangesAsync();
        }
    }
}
