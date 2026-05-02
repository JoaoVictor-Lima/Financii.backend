namespace Financii.Infra.Data.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        Task CommitAsync();

        /// <summary>
        /// Wraps the operation in a single DB transaction.
        /// All CommitAsync calls inside the operation are intermediate flushes within the same transaction.
        /// If any step throws, everything is rolled back atomically.
        /// </summary>
        Task ExecuteInTransactionAsync(Func<Task> operation);
    }
}
