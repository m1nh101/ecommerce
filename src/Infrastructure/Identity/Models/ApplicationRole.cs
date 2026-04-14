using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity.Models;

public sealed class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole() { }
    public ApplicationRole(string roleName) : base(roleName) { }
}
