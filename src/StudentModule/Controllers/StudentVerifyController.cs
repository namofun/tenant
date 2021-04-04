using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SatelliteSite.Services;
using SatelliteSite.StudentModule.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tenant.Entities;
using Tenant.Services;

namespace SatelliteSite.StudentModule.Controllers
{
    [Area("Tenant")]
    [Authorize]
    public class StudentVerifyController : ViewControllerBase
    {
        private readonly IEmailSender _emailSender;
        private readonly IStudentStore _studentStore;
        private readonly IUserManager _userManager;
        private readonly IAffiliationStore _affiliationStore;

        public StudentVerifyController(
            IUserManager userManager,
            IEmailSender emailSender,
            IStudentStore studentStore,
            IAffiliationStore affiliationStore)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _studentStore = studentStore;
            _affiliationStore = affiliationStore;
        }

        private async Task<IUserWithStudent> GetUserAsync()
        {
            var user = (IUserWithStudent)await _userManager.GetUserAsync(User);
            var userId = _userManager.GetUserId(User);
            ViewBag.User = user;
            return user ?? throw new ApplicationException(
                $"Unable to load user with ID '{userId}'.");
        }

        private async Task SendConfirmationEmailsAsync(IUserWithStudent user)
        {
            string code = await _userManager.GenerateUserTokenAsync(
                user: user,
                tokenProvider: EmailTokenProviderConstants.EmailTokenProvider,
                purpose: EmailTokenProviderConstants.EmailTokenPurpose);

            string callbackUrl = Url.Action(
                action: nameof(ConfirmEmail),
                controller: "StudentVerify",
                values: new { userId = user.Id, code, area = "Tenant" },
                protocol: Request.Scheme);

            await _emailSender.SendEmailConfirmationAsync(user.StudentEmail, callbackUrl);
        }


        [HttpGet("/profile/{username}/student-verify")]
        public async Task<IActionResult> Main(string username)
        {
            var user = await GetUserAsync();
            if (!user.HasUserName(username)) return NotFound();
            var affiliations = await _affiliationStore.ListAsync(a => a.EmailSuffix != null);
            ViewBag.Affiliations = affiliations;

            Student student = null;
            Affiliation aff = null;
            string rawStudId = null;
            if (user.StudentId != null)
            {
                var list = user.StudentId.Split('_');
                if (int.TryParse(list[0], out int affId))
                {
                    aff = await _affiliationStore.FindAsync(affId);
                }

                rawStudId = list[1];
                if (aff == null) throw new ApplicationException("Unknown foreign key configured.");
                student = await _studentStore.FindStudentAsync(aff, rawStudId);
            }

            return View(new StudentVerifyModel
            {
                StudentName = student?.Name,
                StudentId = rawStudId,
                Email = user.StudentEmail,
                IsEmailConfirmed = user.StudentVerified,
                AffiliationId = aff?.Id ?? 0,
            });
        }


        [HttpPost("/profile/{username}/student-verify")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Main(string username, StudentVerifyModel model)
        {
            var user = await GetUserAsync();
            if (!user.HasUserName(username)) return NotFound();
            var affiliations = await _affiliationStore.ListAsync(a => a.EmailSuffix != null);
            ViewBag.Affiliations = affiliations;

            if (!ModelState.IsValid) return View(model);

            if (user.StudentVerified)
            {
                StatusMessage = "You have already been verified.";
                return RedirectToAction(nameof(Main));
            }

            if (string.IsNullOrEmpty(model.Email))
            {
                user.StudentId = null;
                user.StudentEmail = null;
                await _userManager.UpdateAsync(user);
                StatusMessage = "Previous verification cleared.";
                return RedirectToAction(nameof(Main));
            }

            Affiliation aff = affiliations.FirstOrDefault(a => a.Id == model.AffiliationId);
            if (aff == null)
            {
                ModelState.AddModelError("XYS.StudentInfo",
                    "The selected school value is invalid. Please check it carefully.");
                return View(model);
            }

            if (!model.Email.EndsWith(aff.EmailSuffix))
            {
                ModelState.AddModelError("XYS.EmailType",
                    "The domain of your student email address is not permitted. " +
                    "If you believe this is a mistake, please contact your tenant administrator.");
                return View(model);
            }

            Student student = await _studentStore.FindStudentAsync(aff, model.StudentId);
            if (student == null || student.Name != model.StudentName.Trim())
            {
                ModelState.AddModelError("XYS.StudentInfo",
                    "Your name or ID is invalid. " +
                    "If you believe this is a mistake, please contact your tenant administrator.");
                return View(model);
            }

            var users2 = await _studentStore.FindUserByStudentAsync(student);
            var users = users2.Cast<IUserWithStudent>().SingleOrDefault(u => u.StudentEmail != null);
            if (users != null && users.Id != user.Id)
            {
                ModelState.AddModelError("XYS.VerifyCount",
                    "Your student has been verified by other account. " +
                    "If you believe this is a mistake, please contact your tenant administrator.");
                return View(model);
            }

            user.StudentEmail = model.Email;
            user.StudentId = student.Id;
            await _userManager.UpdateAsync(user);
            await SendConfirmationEmailsAsync(user);
            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToAction(nameof(Main));
        }


        [HttpPost("/profile/{username}/send-student-email")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendStudentEmail(string username)
        {
            var user = await GetUserAsync();
            if (!user.HasUserName(username)) return NotFound();

            if (user.StudentVerified)
            {
                StatusMessage = "Student already verified.";
                return RedirectToAction(nameof(Main));
            }

            if (user.StudentEmail == null)
            {
                StatusMessage = "Error no student email set.";
                return RedirectToAction(nameof(Main));
            }

            await SendConfirmationEmailsAsync(user);
            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToAction(nameof(Main));
        }


        [HttpGet("/account/verify-student-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(int userId, string code)
        {
            if (code == null) return NotFound();
            var user = (IUserWithStudent)await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            bool verified = await _userManager.VerifyUserTokenAsync(
                user: user,
                tokenProvider: EmailTokenProviderConstants.EmailTokenProvider,
                purpose: EmailTokenProviderConstants.EmailTokenPurpose,
                token: code);

            if (verified)
            {
                user.StudentVerified = true;
                var result = await _userManager.UpdateAsync(user);
                await _userManager.AddToRoleAsync(user, "Student");
                return View(result.Succeeded ? "ConfirmEmail" : "ConfirmEmailError");
            }
            else
            {
                return View("ConfirmEmailError");
            }
        }
    }
}
