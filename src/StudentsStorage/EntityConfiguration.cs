using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelliteSite.IdentityModule.Entities;

namespace Tenant.Entities
{
    public class StudentEntityConfiguration<TUser, TContext> :
        EntityTypeConfigurationSupplier<TContext>,
        IEntityTypeConfiguration<Student>,
        IEntityTypeConfiguration<VerifyCode>,
        IEntityTypeConfiguration<Class>,
        IEntityTypeConfiguration<ClassStudent>
        where TUser : User, IUserWithStudent
        where TContext : DbContext
    {
        public void Configure(EntityTypeBuilder<Student> entity)
        {
            entity.ToTable("TenantStudents");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasMaxLength(32);

            entity.Property(e => e.Name)
                .HasMaxLength(32);

            entity.HasOne<Affiliation>()
                .WithMany()
                .HasForeignKey(e => e.AffiliationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany<TUser>()
                .WithOne()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.Ignore(e => e.UserName);
            entity.Ignore(e => e.Email);
            entity.Ignore(e => e.UserId);
            entity.Ignore(e => e.IsVerified);
        }

        public void Configure(EntityTypeBuilder<Class> entity)
        {
            entity.ToTable("TenantTeachingClasses");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .HasMaxLength(64);

            entity.HasOne<Affiliation>(e => e.Affiliation)
                .WithMany()
                .HasForeignKey(e => e.AffiliationId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(e => e.UserName)
                .HasMaxLength(128);

            entity.HasOne<TUser>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Ignore(e => e.Count);
        }

        public void Configure(EntityTypeBuilder<ClassStudent> entity)
        {
            entity.ToTable("TenantClassStudents");

            entity.HasKey(e => new { e.StudentId, e.ClassId });

            entity.HasOne<Student>()
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<Class>()
                .WithMany(e => e.Students)
                .HasForeignKey(e => e.ClassId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public void Configure(EntityTypeBuilder<VerifyCode> entity)
        {
            entity.ToTable("TenantVerifyCodes");

            entity.HasKey(e => e.Id);

            entity.HasOne<Affiliation>()
                .WithMany()
                .HasForeignKey(e => e.AffiliationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Code);

            entity.Property(e => e.Code)
                .HasMaxLength(16);

            entity.HasOne<TUser>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
