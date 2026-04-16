using Financii.Domain.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Financii.Domain.Entities
{
    public class User : IdentityUser<long>, IEntity
    {
        public User() { }

        public User(string name, string email)
        {
            PublicId = Guid.NewGuid();
            Name = name;
            Email = email;
            UserName = email;
            CreatedAt = DateTime.UtcNow;
        }

        public string Name { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public Guid PublicId { get; set; }
    }
}
