﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using SatelliteSite.IdentityModule.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xylab.Tenant.Entities;
using Xylab.Tenant.Services;

namespace SatelliteSite.StudentModule
{
    public class StudentAdditionalRole : IAdditionalRole
    {
        public string Category { get; }

        public string Title => Text;

        public string Text { get; }

        public string GetUrl(object urlHelper) => null;

        public StudentAdditionalRole(bool valid, string result)
        {
            Category = valid ? "Student" : "Student (?)";
            Text = result;
        }
    }

    public class TenantAdminAdditionalRole : IAdditionalRole
    {
        private readonly int affid;

        public string Category => "Tenant Administrator";

        public string Title => Text;

        public string Text { get; }

        public string GetUrl(object urlHelper)
        {
            return ((IUrlHelper)urlHelper).Action("Detail", "Affiliations", new { area = "Dashboard", affid });
        }

        public TenantAdminAdditionalRole(Affiliation a)
        {
            Text = a.Name;
            affid = a.Id;
        }
    }

    public class StudentAdditionProvider : INotificationHandler<UserDetailModel>
    {
        private readonly IStudentStore _store;

        public StudentAdditionProvider(IStudentStore store)
            => _store = store;

        public async Task Handle(UserDetailModel notification, CancellationToken cancellationToken)
        {
            if (notification.User is IUserWithStudent stu
                && stu.StudentId != null)
            {
                var stud = await _store.FindStudentAsync(stu.StudentId);
                var valid = stu.StudentVerified && stud != null;
                var desc = stud.HasValue
                    ? $"{stud.Value.Item1.Name} - {stud.Value.Item2.Id.Split(new[] { '_' }, 2)[1]}{stud.Value.Item2.Name}"
                    : $"UNKNOWN - {stu.StudentId}";

                notification.AddMore(new StudentAdditionalRole(valid, desc));
            }

            var aff = await _store.GetAffiliationsForUserAsync(notification.User);
            notification.AddMore(aff.Select(a => new TenantAdminAdditionalRole(a)));
        }
    }
}
