namespace Financii.Domain.Contracts
{
    public interface IEntity
    {
        long Id { get; }

        Guid PublicId { get; }
    }
}
