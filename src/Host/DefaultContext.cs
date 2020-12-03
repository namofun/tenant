﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SatelliteSite.IdentityModule.Entities;

namespace SatelliteSite
{
    public class DefaultContext : IdentityDbContext<AspNetUser, Role, int>
    {
        public DefaultContext(DbContextOptions options)
            : base(options)
        {
        }
    }
}
