using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Mailing;
using SatelliteSite.StudentModule.Models;
using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Xylab.Tenant.Entities;
using Xylab.Tenant.Services;

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

            await _emailSender.SendEmailAsync(
                user.StudentEmail,
                "Confirm your student eligibility",
                "Please confirm your student eligibility by clicking this link: <a href='" + HtmlEncoder.Default.Encode(callbackUrl) + "'>link</a>");
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
                PendingConfirm = user.StudentId == null ? default(bool?) : !user.StudentVerified,
                AffiliationId = aff?.Id ?? 0,
            });
        }


        [HttpPost("/profile/{username}/student-verify")]
        [ValidateAntiForgeryToken]
        [AuditPoint(AuditlogType.Student)]
        public async Task<IActionResult> Main(string username, StudentVerifyModel model)
        {
            var user = await GetUserAsync();
            if (!user.HasUserName(username)) return NotFound();
            var affiliations = await _affiliationStore.ListAsync(a => a.EmailSuffix != null);
            ViewBag.Affiliations = affiliations;
            model.PendingConfirm = user.StudentId == null ? default(bool?) : !user.StudentVerified;
            if (model.VerifyOption != 0 && model.VerifyOption != 1) return BadRequest();

            if (user.StudentVerified)
            {
                StatusMessage = "You have already been verified.";
                return RedirectToAction(nameof(Main));
            }

            if (!ModelState.IsValid) return View(model);

            if (user.StudentId == null && string.IsNullOrWhiteSpace(model.StudentId))
                return Fail(
                    "StudentId",
                    "You must fill in the student ID to continue verification.");

            if (model.VerifyOption == 0 && string.IsNullOrWhiteSpace(model.Email))
                return Fail(
                    "Email",
                    "You must fill in the student email address to continue verification.");

            if (model.VerifyOption == 1 && string.IsNullOrWhiteSpace(model.VerifyCode))
                return Fail(
                    "VerifyCode",
                    "You must fill in the verification code to continue verification.");

            if (string.IsNullOrEmpty(model.StudentId))
            {
                user.StudentId = null;
                user.StudentEmail = null;
                user.StudentVerified = false;
                await _userManager.UpdateAsync(user);
                // This time, student is not in Student role.
                // When student is verified and in role, they cannot reach here.
                StatusMessage = "Previous verification cleared.";
                return RedirectToAction(nameof(Main));
            }

            Affiliation aff = affiliations.FirstOrDefault(a => a.Id == model.AffiliationId);
            if (aff == null)
                return Fail(
                    "AffiliationId",
                    "The selected school value is invalid. Please check it carefully.");

            if (model.VerifyOption == 0 && !model.Email.EndsWith(aff.EmailSuffix))
                return Fail(
                    "XYS.VerifyCount",
                    "The domain of your student email address is not permitted. " +
                    "If you believe this is a mistake, please contact your tenant administrator.");

            Student student = await _studentStore.FindStudentAsync(aff, model.StudentId);
            if (student == null || student.Name != model.StudentName.Trim())
                return Fail(
                    "XYS.VerifyCount",
                    "Your name or ID is invalid. " +
                    "If you believe this is a mistake, please contact your tenant administrator.");

            var users2 = await _studentStore.FindUserByStudentAsync(student);
            var users = users2.Cast<IUserWithStudent>().SingleOrDefault(u => u.StudentEmail != null);
            if (users != null && users.Id != user.Id)
                return Fail(
                    "XYS.VerifyCount",
                    "Your student has been verified by other account. " +
                    "If you believe this is a mistake, please contact your tenant administrator.");

            user.StudentEmail = null;
            user.StudentVerified = false;
            user.StudentId = student.Id;

            if (model.VerifyOption == 0)
            {
                user.StudentEmail = model.Email;
            }
            else if (model.VerifyOption == 1)
            {
                if (!await _studentStore.RedeemCodeAsync(aff, model.VerifyCode.Trim()))
                {
                    return Fail(
                        "VerifyCode",
                        "The provided invitation code is invalid. " +
                        "If you believe this is a mistake, please contact your tenant administrator.");
                }
                else
                {
                    user.StudentVerified = true;
                }
            }

            await _userManager.UpdateAsync(user);

            if (model.VerifyOption == 0)
            {
                await SendConfirmationEmailsAsync(user);
                await HttpContext.AuditAsync("send verification email", user.StudentId, "to " + user.StudentEmail);
                StatusMessage = "Verification email sent. Please check your email.";
                return RedirectToAction(nameof(Main));
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Student");
                await HttpContext.AuditAsync("verified", student.Id, "via code " + model.VerifyCode.Trim());
                StatusMessage = "Verification succeeded.";
                return RedirectToAction(nameof(Main));
            }

            IActionResult Fail(string key, string val)
            {
                ModelState.AddModelError(key, val);
                return View(model);
            }
        }


        [HttpPost("/profile/{username}/send-student-email")]
        [ValidateAntiForgeryToken]
        [AuditPoint(AuditlogType.Student)]
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
            await HttpContext.AuditAsync("send verification email", user.StudentId, "to " + user.StudentEmail);
            StatusMessage = "Verification email sent. Please check your email.";
            return RedirectToAction(nameof(Main));
        }


        [HttpGet("/account/verify-student-email")]
        [AllowAnonymous]
        [AuditPoint(AuditlogType.Student)]
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
                await HttpContext.AuditAsync("verified", user.StudentId, "via email " + user.StudentEmail);
                return View(result.Succeeded ? "ConfirmEmail" : "ConfirmEmailError");
            }
            else
            {
                return View("ConfirmEmailError");
            }
        }
    }
}
