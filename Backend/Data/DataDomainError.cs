using TaskApi.Core.Models;

namespace TaskApi.Data;

public static class DataDomainError
{
    public static Error FailedToPersistData(string mesg) => new(nameof(FailedToPersistData), mesg);
    public static Error DuplicateEntry => new(nameof(DuplicateEntry), "Failed to save data: Duplicate entry");
}
