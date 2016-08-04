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

            var characters = _context.Characters
                .Include(character => character.CharacterClass)
                .Include(character => character.CharacterRace)
                .Where(character => character.User == user)
                .ToList();

            var timeZones = new List<SelectListItem>();
            foreach (var tz in TimeZoneInfo.GetSystemTimeZones())
            {
                timeZones.Add(new SelectListItem
                {
                    Text = tz.Id,
                    Value = tz.Id,
                    Selected = (tz.Id == "Europe/Paris")
                });
            }

            var model = new ManageIndexViewModel
            {
                Characters = characters,
                TimeZones = timeZones.OrderBy(tz => tz.Value).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ManageIndexViewModel model)
        {
            var user = await GetCurrentUserAsync();

            user.LocalTimeZoneInfoId = model.LocalTimeZoneInfoId;
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
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
            AddPhoneSuccess,
            AddLoginSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        #endregion
    }
}
