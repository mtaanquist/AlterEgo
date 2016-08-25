using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AlterEgo.Models;
using AlterEgo.Models.ManageViewModels;
using AlterEgo.Services;
using AlterEgo.Data;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace AlterEgo.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger _logger;
        private readonly ApplicationDbContext _context;

        public ManageController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailSender emailSender,
        ISmsSender smsSender,
        ILoggerFactory loggerFactory,
        ApplicationDbContext context
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = loggerFactory.CreateLogger<ManageController>();
            _context = context;
        }

        //
        // GET: /Manage/Index
        [HttpGet]
        public async Task<IActionResult> Index(ManageMessageId? message = null)
        {
            var user = await GetCurrentUserAsync();

            if (string.IsNullOrEmpty(user.AccessToken))
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Login", "Account");
            }

            if (user == null)
            {
                return View("Error");
            }

            var characters = _context.Characters.AsNoTracking()
                .Include(character => character.CharacterClass)
                .Include(character => character.CharacterRace)
                .Where(character => character.UserId == user.Id)
                .ToList();

            var timeZones = TimeZoneInfo.GetSystemTimeZones().Select(tz => new SelectListItem
            {
                Text = tz.Id, Value = tz.Id, Selected = user.LocalTimeZoneInfoId == tz.Id
            }).ToList();

            var model = new ManageViewModel
            {
                Characters = characters,
                TimeZones = timeZones.OrderBy(tz => tz.Value).ToList(),
                HasPassword = await _userManager.HasPasswordAsync(user),
                IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user),
                UserAccessTokenExpiry = user.AccessTokenExpiry
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ManageViewModel model)
        {
            var user = await GetCurrentUserAsync();

            user.LocalTimeZoneInfoId = model.LocalTimeZoneInfoId;
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }

        //
        // GET: /Manage/SetPassword
        [HttpGet]
        public IActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
                return View(model);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        //
        // GET: /Manage/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User changed their password successfully.");
                    return RedirectToAction(nameof(Index), new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                AddErrors(result);
                return View(model);
            }
            return RedirectToAction(nameof(Index), new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Manage/SetMainCharacter
        [HttpPost]
        public async Task<IActionResult> SetMainCharacter(string name, string realm)
        {
            var user = await GetCurrentUserAsync();
            // If the user has an old main character, we need to remove it first
            if (!string.IsNullOrEmpty(user.MainCharacterName))
            {
                user.MainCharacter = null;
                await _userManager.UpdateAsync(user);

                var oldMainCharacter =
                    _context.Characters.FirstOrDefault(
                        c => c.Name == user.MainCharacterName && c.Realm == user.MainCharacterRealm);

                oldMainCharacter.MainCharacterUser = null;
                oldMainCharacter.MainCharacterUserId = null;
                _context.Update(oldMainCharacter);
                _context.SaveChanges();
            }

            var character = _context.Characters.FirstOrDefault(c => c.Name == name && c.Realm == realm);

            user.MainCharacter = character;
            user.MainCharacterName = character.Name;
            user.MainCharacterRealm = character.Realm;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Ok();

            return BadRequest();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            Error
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        #endregion
    }
}
