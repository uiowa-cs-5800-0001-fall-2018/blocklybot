using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BlockBot.Common.Data;
using BlockBot.Module.Google.Models;
using BlockBot.Web.Models.AccountManage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BlockBot.Web.Controllers
{
    public class AccountManageController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger<AccountManageController> _logger;
        private readonly ApplicationSignInManager _signInManager;
        private readonly UrlEncoder _urlEncoder;
        private readonly ApplicationUserManager _userManager;

        public AccountManageController(
            ApplicationUserManager userManager,
            ApplicationSignInManager signInManager,
            UrlEncoder urlEncoder,
            ILogger<AccountManageController> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _urlEncoder = urlEncoder;
            _logger = logger;
            _emailSender = emailSender;
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            bool hasPassword = await _userManager.HasPasswordAsync(user);
            if (!hasPassword)
            {
                return RedirectToAction("SetPassword");
            }

            return View("ChangePassword", new ChangePasswordModel());
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ChangePassword", model);
            }

            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            IdentityResult changePasswordResult =
                await _userManager.ChangePasswordAsync(user, model.Input.OldPassword, model.Input.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (IdentityError error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View("ChangePassword", model);
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            TempData["StatusMessage"] = "Your password has been changed.";

            // TODO verify this redirects to the GET version of this page
            return RedirectToAction();
        }

        [HttpGet]
        public async Task<IActionResult> DeletePersonalData()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            DeletePersonalDataModel model = new DeletePersonalDataModel
                {RequirePassword = await _userManager.HasPasswordAsync(user)};

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeletePersonalData(DeletePersonalDataModel model)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            bool requirePassword = await _userManager.HasPasswordAsync(user);
            if (requirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, model.Input.Password))
                {
                    ModelState.AddModelError(string.Empty, "Password not correct.");
                    return View("DeletePersonalData", model);
                }
            }

            IdentityResult result = await _userManager.DeleteAsync(user);
            string userId = await _userManager.GetUserIdAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user with ID '{userId}'.");
            }

            await _signInManager.SignOutAsync();

            _logger.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            return Redirect("~/");
        }

        [HttpGet]
        public async Task<IActionResult> Disable2fa()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                throw new InvalidOperationException(
                    $"Cannot disable 2FA for user with ID '{_userManager.GetUserId(User)}' as it's not currently enabled.");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Disable2fa(string unusedString = null)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            IdentityResult disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Unexpected error occurred disabling 2FA for user with ID '{_userManager.GetUserId(User)}'.");
            }

            _logger.LogInformation("User with ID '{UserId}' has disabled 2fa.", _userManager.GetUserId(User));
            TempData["StatusMessage"] =
                "2fa has been disabled. You can reenable 2fa when you setup an authenticator app.";
            return RedirectToAction("TwoFactorAuthentication");
        }

        [HttpPost]
        public async Task<IActionResult> DownloadPersonalData()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            _logger.LogInformation("User with ID '{UserId}' asked for their personal data.",
                _userManager.GetUserId(User));

            // Only include personal data for download
            Dictionary<string, string> personalData = new Dictionary<string, string>();
            IEnumerable<PropertyInfo> personalDataProps = typeof(IdentityUser).GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));
            foreach (PropertyInfo p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
            return new FileContentResult(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(personalData)),
                "text/json");
        }

        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            EnableAuthenticatorModel model = await LoadSharedKeyAndQrCodeUri(user, new EnableAuthenticatorModel());

            return View("EnableAuthenticator", model);
        }

        [HttpPost]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorModel model)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                model = await LoadSharedKeyAndQrCodeUri(user, model);
                return View("EnableAuthenticator", model);
            }

            // Strip spaces and hypens
            string verificationCode = model.Input.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            bool is2faTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2faTokenValid)
            {
                ModelState.AddModelError("Input.Code", "Verification code is invalid.");
                model = await LoadSharedKeyAndQrCodeUri(user, model);
                return View("EnableAuthenticator", model);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            string userId = await _userManager.GetUserIdAsync(user);
            _logger.LogInformation("User with ID '{UserId}' has enabled 2FA with an authenticator app.", userId);

            TempData["StatusMessage"] = "Your authenticator app has been verified.";

            if (await _userManager.CountRecoveryCodesAsync(user) == 0)
            {
                IEnumerable<string> recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
                TempData["RecoveryCodes"] = recoveryCodes.ToArray();
                return RedirectToAction("ShowRecoveryCodes");
            }

            return RedirectToAction("TwoFactorAuthentication");
        }

        private async Task<EnableAuthenticatorModel> LoadSharedKeyAndQrCodeUri(ApplicationUser user,
            EnableAuthenticatorModel model)
        {
            // Load the authenticator key & QR code URI to display on the form
            string unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            model.SharedKey = FormatKey(unformattedKey);

            string email = await _userManager.GetEmailAsync(user);
            model.AuthenticatorUri = GenerateQrCodeUri(email, unformattedKey);
            return model;
        }

        private string FormatKey(string unformattedKey)
        {
            StringBuilder result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }

            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string email, string unformattedKey)
        {
            return string.Format(
                "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6",
                _urlEncoder.Encode("BlockBot.Web"),
                _urlEncoder.Encode(email),
                unformattedKey);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLogins()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            IList<UserLoginInfo> currentLogins = await _userManager.GetLoginsAsync(user);
            ExternalLoginsModel model = new ExternalLoginsModel
            {
                CurrentLogins = currentLogins,
                OtherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                    .Where(auth => currentLogins.All(ul => auth.Name != ul.LoginProvider))
                    .ToList(),
                ShowRemoveButton = user.PasswordHash != null || currentLogins.Count > 1
            };

            return View("ExternalLogins", model);
        }

        [HttpPost]
        public async Task<IActionResult> ExternalLoginsRemoveLogin(string loginProvider, string providerKey)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            IdentityResult result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
            if (!result.Succeeded)
            {
                string userId = await _userManager.GetUserIdAsync(user);
                throw new InvalidOperationException(
                    $"Unexpected error occurred removing external login for user with ID '{userId}'.");
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["StatusMessage"] = "The external login was removed.";
            return RedirectToAction("ExternalLogins");
        }

        public async Task<IActionResult> ExternalLoginsLinkLogin(string provider, string returnUrl = null)
        {
            if (returnUrl != null)
            {
                TempData["returnUrl"] = returnUrl;
            }
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            string redirectUrl = Url.Action("ExternalLoginsLinkLoginCallback");
            AuthenticationProperties properties =
                _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl,
                    _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginsLinkLoginCallback(string returnUrl = null)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            ExternalLoginInfo info =
                await _signInManager.GetExternalLoginInfoAsync(await _userManager.GetUserIdAsync(user));
            if (info == null)
            {
                throw new InvalidOperationException(
                    $"Unexpected error occurred loading external login info for user with ID '{user.Id}'.");
            }

            await _userManager.RemoveLoginAsync(user, info.LoginProvider, info.ProviderKey);

            IdentityResult result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(
                    $"Unexpected error occurred adding external login for user with ID '{user.Id}'.");
            }

            var refreshTokenClaim = info.AuthenticationTokens.FirstOrDefault(x => x.Name == "refresh_token");
            if (refreshTokenClaim != null)
            {
                var claims = _userManager.GetClaimsAsync(user).Result.Where(x => x.Type == info.LoginProvider + "RefreshToken").ToList();
                if (claims.Any())
                {
                    foreach (Claim claim in claims)
                    {
                        await _userManager.RemoveClaimAsync(user, claim);
                    }
                    
                }
                await _userManager.AddClaimAsync(user, new Claim(info.LoginProvider + "RefreshToken", refreshTokenClaim.Value, ClaimValueTypes.String, info.LoginProvider));
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            

            TempData["StatusMessage"] = "The external login was added.";
            return RedirectToAction("ExternalLogins");
        }

        [HttpGet]
        public async Task<IActionResult> GenerateRecoveryCodes()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            bool isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            if (!isTwoFactorEnabled)
            {
                string userId = await _userManager.GetUserIdAsync(user);
                throw new InvalidOperationException(
                    $"Cannot generate recovery codes for user with ID '{userId}' because they do not have 2FA enabled.");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GenerateRecoveryCodes(string unusedString = null)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            bool isTwoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);
            string userId = await _userManager.GetUserIdAsync(user);
            if (!isTwoFactorEnabled)
            {
                throw new InvalidOperationException(
                    $"Cannot generate recovery codes for user with ID '{userId}' as they do not have 2FA enabled.");
            }

            IEnumerable<string> recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
            TempData["RecoveryCodes"] = recoveryCodes.ToArray();

            _logger.LogInformation("User with ID '{UserId}' has generated new 2FA recovery codes.", userId);
            TempData["StatusMessage"] = "You have generated new recovery codes.";
            return RedirectToAction("ShowRecoveryCodes");
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            string userName = await _userManager.GetUserNameAsync(user);
            string email = await _userManager.GetEmailAsync(user);
            string phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            IndexModel model = new IndexModel
            {
                Username = userName,
                Input = new IndexModel.InputModel {Email = email, PhoneNumber = phoneNumber},
                IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user)
            };

            return View("Index", model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(IndexModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            string email = await _userManager.GetEmailAsync(user);
            if (model.Input.Email != email)
            {
                IdentityResult setEmailResult = await _userManager.SetEmailAsync(user, model.Input.Email);
                if (!setEmailResult.Succeeded)
                {
                    string userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException(
                        $"Unexpected error occurred setting email for user with ID '{userId}'.");
                }
            }

            string phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (model.Input.PhoneNumber != phoneNumber)
            {
                IdentityResult setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    string userId = await _userManager.GetUserIdAsync(user);
                    throw new InvalidOperationException(
                        $"Unexpected error occurred setting phone number for user with ID '{userId}'.");
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["StatusMessage"] = "Your profile has been updated";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> IndexSendVerificationEmail(IndexModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            string userId = await _userManager.GetUserIdAsync(user);
            string email = await _userManager.GetEmailAsync(user);
            string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string callbackUrl = Url.Action("ConfirmEmail", "Account", new {userId, code});
            await _emailSender.SendEmailAsync(
                email,
                "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            TempData["StatusMessage"] = "Verification email sent. Please check your email.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> PersonalData()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ResetAuthenticator()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetAuthenticator(string unusedString = null)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await _userManager.SetTwoFactorEnabledAsync(user, false);
            await _userManager.ResetAuthenticatorKeyAsync(user);
            _logger.LogInformation("User with ID '{UserId}' has reset their authentication app key.", user.Id);

            await _signInManager.RefreshSignInAsync(user);
            TempData["StatusMessage"] =
                "Your authenticator app key has been reset, you will need to configure your authenticator app using the new key.";

            return RedirectToAction("EnableAuthenticator");
        }

        [HttpGet]
        public async Task<IActionResult> SetPassword()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            bool hasPassword = await _userManager.HasPasswordAsync(user);

            if (hasPassword)
            {
                return RedirectToAction("ChangePassword");
            }

            return View("SetPassword", new SetPasswordModel());
        }

        [HttpPost]
        public async Task<IActionResult> SetPassword(SetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("SetPassword", model);
            }

            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            IdentityResult addPasswordResult = await _userManager.AddPasswordAsync(user, model.Input.NewPassword);
            if (!addPasswordResult.Succeeded)
            {
                foreach (IdentityError error in addPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View("SetPassword", model);
            }

            await _signInManager.RefreshSignInAsync(user);
            TempData["StatusMessage"] = "Your password has been set.";

            return RedirectToAction("SetPassword");
        }

        [HttpGet]
        public IActionResult ShowRecoveryCodes()
        {
            if (!(TempData["RecoveryCodes"] is string[] recoveryCodes) || recoveryCodes.Length == 0)
            {
                return RedirectToAction("TwoFactorAuthentication");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TwoFactorAuthentication()
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            TwoFactorAuthenticationModel model = new TwoFactorAuthenticationModel
            {
                HasAuthenticator = await _userManager.GetAuthenticatorKeyAsync(user) != null,
                Is2faEnabled = await _userManager.GetTwoFactorEnabledAsync(user),
                IsMachineRemembered = await _signInManager.IsTwoFactorClientRememberedAsync(user),
                RecoveryCodesLeft = await _userManager.CountRecoveryCodesAsync(user)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> TwoFactorAuthentication(TwoFactorAuthenticationModel model)
        {
            ApplicationUser user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await _signInManager.ForgetTwoFactorClientAsync();
            TempData["StatusMessage"] =
                "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.";
            return RedirectToAction();
        }
    }
}