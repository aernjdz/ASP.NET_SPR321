using Microsoft.AspNetCore.Identity;

namespace DataAcess.Data.Entities.Identity
{
    public class RoleEntity : IdentityRole<int>
    {
        public ICollection<UserRoleEntity> Roles { get; set; } = [];
    }
}
