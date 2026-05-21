using TaskApi.Core.Models;

namespace TaskApi.Core;

public record ApiResponse(
    string? Message,
    string? ErrorCode,
    string? ErrorDescription,
    string[]? Errors = null
)
{
    public static implicit operator ApiResponse(Result result) => result.IsSuccess
        ? new ApiResponse(result.Message, null, null)
        : new ApiResponse(null, result.Error.Code, result.Error.Description);
}

public record ApiResponse<TData>(
    string? Message,
    TData? Data,
    string? ErrorCode,
    string? ErrorDescription,
    string[]? Errors = null
)
{
    public static implicit operator ApiResponse<TData>(Result<TData> result) => result.IsSuccess
        ? new ApiResponse<TData>(result.Message, result.Value, null, null)
        : new ApiResponse<TData>(null, default, result.Error.Code, result.Error.Description);
}
