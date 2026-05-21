using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using TaskApi.Core;
using TaskApi.Core.Models;
using TaskApi.Data;

namespace TaskApi.Features.TaskModule.Endpoints;

public record DeleteTaskRequest(int Id);

public class DeleteTaskEndpoint(
    AppDbContext _dbContext,
    UnitOfWork _unitOfWork
) : Endpoint<DeleteTaskRequest, ApiResponse>
{
    public override void Configure()
    {
        Delete("{id}");
        Group<TaskModuleEndpointGroup>();
    }

    public override async Task HandleAsync(DeleteTaskRequest req, CancellationToken ct)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var task = await _dbContext.Tasks
            .FirstOrDefaultAsync(x => x.Id == req.Id && x.UserId == int.Parse(userId), ct);

        if (task is null)
        {
            var error = CrudDomainError.NotFound("TaskItem", req.Id);
            await Send.ResultAsync(TypedResults.BadRequest<ApiResponse>((Result)error));
            return;
        }

        _dbContext.Tasks.Remove(task);
        var result = await _unitOfWork.SaveChangesAsync(ct);

        if (result.IsFailure)
        {
            await Send.ResultAsync(TypedResults.BadRequest<ApiResponse>(result));
            return;
        }

        await Send.OkAsync(Result.Success(), cancellation: ct);
    }
}
