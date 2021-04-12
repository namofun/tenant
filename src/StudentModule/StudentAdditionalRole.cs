using MediatR;
using SatelliteSite.IdentityModule.Models;
using System.Threading;
using System.Threading.Tasks;
using Tenant.Entities;
using Tenant.Services;

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

    public class StudentAdditionProvider : INotificationHandler<UserDetailModel>
    {
        private readonly IStudentStore _store;

        public StudentAdditionProvider(IStudentStore store)
            => _store = store;

        public async Task Handle(UserDetailModel notification, CancellationToken cancellationToken)
        {
            if (!(notification.User is IUserWithStudent stu)
                || stu.StudentId == null)
                return;

            var stud = await _store.FindStudentAsync(stu.StudentId);
            var valid = stu.StudentVerified && stud != null;
            var desc = stud.HasValue
                ? $"{stud.Value.Item1.Name} - {stud.Value.Item2.Id.Split(new[] { '_' }, 2)[1]}{stud.Value.Item2.Name}"
                : $"UNKNOWN - {stu.StudentId}";

            notification.AddMore(new StudentAdditionalRole(valid, desc));
        }
    }
}
