using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;

namespace Xylab.Tenant.Entities
{
    public class TrainingTeamEntityConfiguration<TContext> :
        EntityTypeConfigurationSupplier<TContext>,
        IEntityTypeConfiguration<GroupTeam>,
        IEntityTypeConfiguration<GroupUser>
        where TContext : DbContext
    {
        private static readonly Lazy<Type?> _lazyTyper
            = new Lazy<Type?>(() =>
            {
                var usersProp = typeof(TContext).GetProperty("Users");
                if (usersProp == null) return null;
                var usersDbSetType = usersProp.PropertyType;
                if (!usersDbSetType.IsConstructedGenericType) return null;
                if (usersDbSetType.GetGenericTypeDefinition() != typeof(DbSet<>)) return null;
                var userType = usersDbSetType.GetGenericArguments().Single();
                if (!typeof(Microsoft.AspNetCore.Identity.IUser).IsAssignableFrom(userType)) return null;
                return userType;
            });

        public static Type? UserType => _lazyTyper.Value;

        public void Configure(EntityTypeBuilder<GroupTeam> entity)
        {
            entity.ToTable("TenantTrainingTeams");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.TeamName)
                .IsRequired()
                .HasMaxLength(128);

            entity.HasOne<Affiliation>(e => e.Affiliation)
                .WithMany()
                .HasForeignKey(e => e.AffiliationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(UserType!)
                .WithMany()
                .HasForeignKey(nameof(GroupTeam.UserId))
                .OnDelete(DeleteBehavior.Restrict);
        }

        public void Configure(EntityTypeBuilder<GroupUser> entity)
        {
            entity.ToTable("TenantTrainingUsers");

            entity.HasKey(e => new { e.TeamId, e.UserId });

            entity.HasOne<GroupTeam>()
                .WithMany(e => e.Users)
                .HasForeignKey(e => e.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(UserType!)
                .WithMany()
                .HasForeignKey(nameof(GroupUser.UserId))
                .OnDelete(DeleteBehavior.Cascade);

            entity.Ignore(e => e.UserName);
            entity.Ignore(e => e.UserEmail);
        }
    }
}
