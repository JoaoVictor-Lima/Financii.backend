namespace Financii.Application.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }
}
