using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TaskApi.Core;
using TaskApi.Core.Models;
using TaskApi.Data;

namespace TaskApi.Features.TaskModule.Endpoints;

public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.");
    }
}

public record UpdateTaskRequest(
    int Id,
    string Title,
    string Description,
    string Status
);

public record UpdateTaskResponse(
    int Id,
    string Title,
    string Description,
    string Status,
    DateTime CreateTime
);

public class UpdateTaskEndpoint(
    AppDbContext dbContext,
    UnitOfWork unitOfWork
) : Endpoint<UpdateTaskRequest, ApiResponse<UpdateTaskResponse>>
{
    public override void Configure()
    {
        Put("{id}");
        Group<TaskModuleEndpointGroup>();
    }

    public override async Task HandleAsync(UpdateTaskRequest req, CancellationToken ct)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var task = await dbContext.Tasks.FirstOrDefaultAsync(x => x.Id == req.Id && x.UserId == int.Parse(userId), ct);

        if (task is null)
        {
            var error = CrudDomainError.NotFound(nameof(TaskItem), req.Id);
            await Send.ResultAsync(TypedResults.BadRequest<ApiResponse>((Result)error));
            return;
        }

        task.Update(req.Title, req.Description, req.Status);

        var result = await unitOfWork.SaveChangesAsync(ct);

        if (result.IsFailure)
        {
            await Send.ResultAsync(TypedResults.BadRequest<ApiResponse>(result));
            return;
        }

        var response = new UpdateTaskResponse(
            task.Id,
            task.Title,
            task.Description,
            task.TaskStatus,
            task.CreateTime
        );

        await Send.OkAsync(Result.Success(response), cancellation: ct);
    }
}
