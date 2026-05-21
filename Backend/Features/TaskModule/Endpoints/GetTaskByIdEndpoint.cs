using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using TaskApi.Core;
using TaskApi.Core.Models;
using TaskApi.Data;

namespace TaskApi.Features.TaskModule.Endpoints;

public record GetByIdTaskRequest(int Id);

public record GetByIdTaskResponse(
    int Id,
    string Title,
    string Description,
    string Status,
    DateTime CreateTime
);

public class GetTaskByIdEndpoint(AppDbContext _dbContext) : Endpoint<GetByIdTaskRequest, ApiResponse<GetByIdTaskResponse>>
{
    public override void Configure()
    {
        Get("{id}");
        Group<TaskModuleEndpointGroup>();
    }

    public override async Task HandleAsync(GetByIdTaskRequest req, CancellationToken ct)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var task = await _dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == req.Id && x.UserId == int.Parse(userId), ct);

        if (task is null)
        {
            var error = CrudDomainError.NotFound(nameof(TaskItem), req.Id);
            await Send.ResultAsync(TypedResults.BadRequest<ApiResponse>((Result)error));
            return;
        }

        var response = new GetByIdTaskResponse(
            task.Id,
            task.Title,
            task.Description,
            task.TaskStatus,
            task.CreateTime
        );

        await Send.OkAsync(Result.Success(response), cancellation: ct);
    }
}
