using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xylab.Tenant.Entities;
using Xylab.Tenant.Services;

namespace SatelliteSite.StudentModule
{
    public class StudentTenantClaimsProvider : IUserClaimsProvider
    {
        public IStudentStore Store { get; }

        public StudentTenantClaimsProvider(IStudentStore store)
            => Store = store;

        public async Task<IEnumerable<Claim>> GetClaimsAsync(IUser _user)
        {
            var result = Enumerable.Empty<Claim>();
            if (_user is IUserWithStudent user && user.StudentVerified)
            {
                var res = await Store.FindStudentAsync(user.StudentId);
                if (res is (Affiliation a, Student s))
                {
                    var affId = a.Id.ToString();
                    var stuId = s.Id[(affId.Length + 1)..];

                    result = new[]
                    {
                        new Claim("tenant", affId),
                        new Claim("student", stuId),
                        new Claim("affiliation", a.Name),
                        new Claim("given_name", s.Name),
                    };
                }
            }

            return result;
        }
    }
}
