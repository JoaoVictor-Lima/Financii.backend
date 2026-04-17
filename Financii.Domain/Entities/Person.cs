using Financii.Domain.Contracts;

namespace Financii.Domain.Entities
{
    public class Person : EntityBase
    {
        protected Person() { }

        public Person(long userId, string name, string cpf, DateTime birthDate)
        {
            PublicId = Guid.NewGuid();
            UserId = userId;
            Name = name;
            Cpf = cpf;
            BirthDate = birthDate;
        }

        public long UserId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Cpf { get; private set; } = string.Empty;
        public DateTime BirthDate { get; private set; }
    }
}
