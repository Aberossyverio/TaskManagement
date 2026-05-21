using TaskApi.Core.Models;

namespace TaskApi.Features.TaskModule;

public class TaskItem : AuditableEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string TaskStatus { get; set; }
    public int UserId { get; set; }

#pragma warning disable CS8618
    public TaskItem() { }
#pragma warning restore CS8618

    private TaskItem(string title, string description, string taskStatus, int userId)
    {
        Title = title;
        Description = description;
        TaskStatus = taskStatus;
        UserId = userId;
    }

    public void Update(string title, string description, string taskStatus)
    {
        Title = title;
        Description = description;
        TaskStatus = taskStatus;
    }

    public static TaskItem Create(string title, string description, string taskStatus, int userId)
    {
        return new TaskItem(title, description, taskStatus, userId);
    }
}
