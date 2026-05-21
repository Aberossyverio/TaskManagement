using FastEndpoints;
using TaskApi.Core.Paging;
using TaskApi.Data;

namespace TaskApi.Features.TaskModule.Endpoints;

public record GetTaskRequest(
    string? Title = null,
    string? Status = null
) : PagingQuery;

public class GetAllTasksEndpoint(AppDbContext dbContext)
    : Endpoint<GetTaskRequest, PagingResult<TaskItem>>
{
    public override void Configure()
    {
        Get("");
        Group<TaskModuleEndpointGroup>();
    }

    public override async Task HandleAsync(GetTaskRequest req, CancellationToken ct)
    {
        var userId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            await Send.UnauthorizedAsync(ct);
            return;
        }

        var query = dbContext.Tasks.Where(x => x.UserId == int.Parse(userId)).AsQueryable();

        var predicate = PredicateBuilder.True<TaskItem>();

        if (!string.IsNullOrWhiteSpace(req.Search))
        {
            var searchLower = req.Search.ToLower();
            predicate = predicate.And(x =>
                x.Title.ToLower().Contains(searchLower) ||
                x.Description.ToLower().Contains(searchLower)
            );
        }

        if (!string.IsNullOrWhiteSpace(req.Title))
        {
            var titleLower = req.Title.ToLower();
            predicate = predicate.And(x => x.Title.ToLower().Contains(titleLower));
        }

        if (!string.IsNullOrWhiteSpace(req.Status))
        {
            predicate = predicate.And(x => x.TaskStatus == req.Status);
        }

        query = query.Where(predicate);

        var result = await PagingService.PaginateQueryAsync(query, req, dbContext, ct);

        await Send.OkAsync(result, cancellation: ct);
    }
}
