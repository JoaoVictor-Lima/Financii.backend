using Financii.Domain.Contracts;

namespace Financii.Domain.Entities
{
    public class Contribution : EntityBase
    {
        protected Contribution() { }

        public Contribution(long goalId, long personId, decimal amount, DateTime date, string? notes = null)
        {
            PublicId = Guid.NewGuid();
            GoalId = goalId;
            PersonId = personId;
            Amount = amount;
            Date = date;
            Notes = notes;
        }

        public long GoalId { get; private set; }
        public long PersonId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime Date { get; private set; }
        public string? Notes { get; private set; }
    }
}
