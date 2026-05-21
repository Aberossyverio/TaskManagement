using TaskApi.Core.Models;

namespace TaskApi.Features.TaskModule;

public static class TaskModuleDomainError
{
    public static Error InvalidStatus(string status)
        => new(nameof(InvalidStatus), $"Invalid task status: {status}. Valid statuses are: todo, in-progress, done.");

    public static Error TaskAlreadyCompleted(int taskId)
        => new(nameof(TaskAlreadyCompleted), $"Task {taskId} is already completed and cannot be modified.");
}
