using Microsoft.EntityFrameworkCore;
using Npgsql;
using TaskApi.Core.Models;

namespace TaskApi.Data;

public class UnitOfWork(AppDbContext dbContext)
{
    private int _transactionCount = 0;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        if (_transactionCount == 0)
        {
            await dbContext.Database.BeginTransactionAsync(cancellationToken);
        }
        _transactionCount++;
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        if (_transactionCount == 0)
        {
            return;
        }

        _transactionCount--;
        
        if (_transactionCount == 0)
        {
            await dbContext.Database.CommitTransactionAsync(cancellationToken);
        }
    }

    public async Task RollBackTransactionAsync(CancellationToken cancellationToken)
    {
        if (_transactionCount == 0)
        {
            return;
        }

        _transactionCount = 0; // Reset count since we're rolling back everything
        await dbContext.Database.RollbackTransactionAsync(cancellationToken);
    }

    public async Task<Result> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await BeginTransactionAsync(cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);
            
            // Save changes made by event handlers
            var hasChanges = dbContext.ChangeTracker.Entries()
                .Any(e => e.State == EntityState.Added || 
                         e.State == EntityState.Modified || 
                         e.State == EntityState.Deleted);
            
            if (hasChanges)
            {
                var result = await SaveChangesAsync(cancellationToken);
                if (result.IsFailure)
                {
                    return result;
                }
            }
            
            await CommitTransactionAsync(cancellationToken);
            return Result.Success();
        }
        catch (DbUpdateException e)
        {
            await RollBackTransactionAsync(cancellationToken);
            if (e.InnerException is NpgsqlException npgsqlException)
            {
                // PostgreSQL unique constraint violation error code is 23505
                if (npgsqlException.SqlState == "23505")
					return Result.Failure(DataDomainError.DuplicateEntry);
            }

            return Result
                .Failure(new Error("DbUpdateException", e.InnerException?.Message ?? e.Message));
        }
        catch (Exception e)
        {
            await RollBackTransactionAsync(cancellationToken);
            return Result
                .Failure(new Error("DbSaveException", e.InnerException?.Message ?? e.Message));
        }
    }
}
