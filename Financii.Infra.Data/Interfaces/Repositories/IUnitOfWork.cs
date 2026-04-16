namespace Financii.Infra.Data.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }
}
