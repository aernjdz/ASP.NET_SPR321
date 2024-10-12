using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DataAcess.Data.Entities.Identity
{
    public class UserEntity : IdentityUser<int>
    {
        [Required, StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        [StringLength(100)]
        public string? Image { get; set; }
        public ICollection<UserRoleEntity> UserRoles { get; set; } = [];
    }
}
