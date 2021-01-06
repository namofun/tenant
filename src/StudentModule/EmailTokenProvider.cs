using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SatelliteSite.IdentityModule.Services;
using System.Threading.Tasks;
using Tenant.Entities;

namespace SatelliteSite.StudentModule
{
    public class StudentEmailTokenProvider<TUser> :
        TotpSecurityStampBasedTokenProvider<TUser>
        where TUser : class, IUser, IUserWithStudent
    {
        public override async Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<TUser> manager, TUser user)
        {
            var email = await manager.GetEmailAsync(user);
            return !string.IsNullOrWhiteSpace(email) && await manager.IsEmailConfirmedAsync(user);
        }

        public override Task<string> GetUserModifierAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            return Task.FromResult("StudentEmail:" + purpose + ":" + user.StudentEmail);
        }
    }

    public static class EmailTokenProviderConstants
    {
        public const string EmailTokenProvider = "StudentEmail";
        public const string EmailTokenPurpose = "StudentEmailConfirmation";

        public static IServiceCollection AddStudentEmailTokenProvider<TUser>(
            this IServiceCollection services)
            where TUser : class, IUser, IUserWithStudent
        {
            services.Configure<IdentityOptions>(options =>
                options.Tokens.ProviderMap[EmailTokenProvider] =
                    new TokenProviderDescriptor(typeof(StudentEmailTokenProvider<TUser>)));
            services.AddTransient<StudentEmailTokenProvider<TUser>>();
            return services;
        }
    }
}
