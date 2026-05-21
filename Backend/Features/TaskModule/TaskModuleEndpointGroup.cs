using FastEndpoints;

namespace TaskApi.Features.TaskModule;

public class TaskModuleEndpointGroup : Group
{
    public TaskModuleEndpointGroup()
    {
        Configure("tasks", ep =>
        {
            ep.Tags("Task Module");
        });
    }
}
