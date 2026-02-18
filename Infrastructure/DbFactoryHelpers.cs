using Microsoft.EntityFrameworkCore;

namespace DddBlazorDemo.Infrastructure;

public static class DbFactoryHelpers
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="factory"></param>
    /// <param name="action"></param>
    /// <param name="cancellation"></param>
    /// <returns></returns>
    public static async Task<T> RunWithDbContext<T>(this IDbContextFactory<AppDbContext> factory,
      Func<AppDbContext, Task<T>> action,
      CancellationToken cancellation)
    {

        await using var db = await factory.CreateDbContextAsync(cancellation);
        return await action(db);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="action"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public static async Task RunWithDbContext(
        this IDbContextFactory<AppDbContext> factory,
        Func<AppDbContext, Task> action,
        CancellationToken ct = default)
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        await action(db);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="factory"></param>
    /// <param name="action"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public static async Task<T> WithTransaction<TContext, T>(
        this IDbContextFactory<TContext> factory,
        Func<TContext, Task<T>> action,
        CancellationToken ct = default)
        where TContext : DbContext
    {
        await using var db = await factory.CreateDbContextAsync(ct);
        await using var tx = await db.Database.BeginTransactionAsync(ct);

        var result = await action(db);

        await tx.CommitAsync(ct);
        return result;
    }
}