using Microsoft.AspNetCore.Identity;

namespace Financii.Domain.Entities
{
    public class User : IdentityUser<long>
    {
        public string Name { get; set; } = string.Empty;
    }
}
