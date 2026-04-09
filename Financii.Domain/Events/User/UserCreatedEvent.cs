namespace Financii.Domain.Events.User
{
    public record UserCreatedEvent(long UserId) : IDomainEvent;
}
