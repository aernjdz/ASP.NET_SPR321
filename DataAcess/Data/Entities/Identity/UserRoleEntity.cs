﻿using Microsoft.AspNetCore.Identity;

namespace DataAcess.Data.Entities.Identity
{
    public class UserRoleEntity : IdentityUserRole<int>
    {
        public UserEntity User { get; set; } = new();
        public RoleEntity Role { get; set; } = new();
    }
}