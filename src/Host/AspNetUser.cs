﻿using SatelliteSite.IdentityModule.Entities;
using Xylab.Tenant.Entities;

namespace SatelliteSite
{
    public class AspNetUser : User, IUserWithStudent
    {
        public string StudentId { get; set; }
        public string StudentEmail { get; set; }
        public bool StudentVerified { get; set; }
    }
}
