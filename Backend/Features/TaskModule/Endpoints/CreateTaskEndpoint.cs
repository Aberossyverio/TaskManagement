using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TaskApi.Core;
using TaskApi.Core.Models;
using TaskApi.Data;

namespace TaskApi.Features.TaskModule.Endpoints;

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.");
    }
}

public record CreateTaskRequest(
    string Title,
    string Description,
    string Status
);

public record CreateTaskResponse(
    int Id,
    string Title,
    string Description,
    string Status,
    DateTime CreateTime
);

public class CreateTaskEndpoint(
    AppDbContext _dbContext,
    UnitOfWork _unitOfWork
) : Endpoint<CreateTaskRequest, ApiResponse<CreateTaskResponse>>
{
    public override void Configure()
    {
        Post("");
        Group<TaskModuleEndpointGroup>();
    }

    public override async Task HandleAsync(CreateTaskRequest req, CancellationToken ct)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        if (await _dbContext.Tasks.AnyAsync(x => x.Title == req.Title && x.UserId == int.Parse(userId), ct))
        {
            await Send.ResultAsync(TypedResults.Conflict<ApiResponse>((Result)CrudDomainError.Duplicate("Task", "Title")));
            return;
        }

        var task = TaskItem.Create(req.Title, req.Description, req.Status, int.Parse(userId));

        _dbContext.Tasks.Add(task);
        var result = await _unitOfWork.SaveChangesAsync(ct);

        if (result.IsFailure)
        {
            await Send.ResultAsync(TypedResults.BadRequest<ApiResponse>(result));
            return;
        }

        await Send.OkAsync(Result.Success(
            new CreateTaskResponse(
                task.Id,
                task.Title,
                task.Description,
                task.TaskStatus,
                task.CreateTime
            )
        ), cancellation: ct);
    }
}
