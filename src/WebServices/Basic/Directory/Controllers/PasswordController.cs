using System;
using System.Linq;
using System.Threading.Tasks;
using Aiursoft.Canon;
using Aiursoft.Directory.Data;
using Aiursoft.Directory.Models;
using Aiursoft.Directory.Models.PasswordViewModels;
using Aiursoft.Directory.Services;
using Aiursoft.CSTools.Tools;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Aiursoft.Directory.Controllers;


public class PasswordController : Controller
{
    private readonly CanonService _cannonService;
    private readonly DirectoryDbContext _dbContext;
    private readonly ILogger _logger;
    private readonly UserManager<DirectoryUser> _userManager;

    public PasswordController(
        DirectoryDbContext dbContext,
        ILoggerFactory loggerFactory,
        UserManager<DirectoryUser> userManager,
        CanonService cannonService)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _cannonService = cannonService;
        _logger = loggerFactory.CreateLogger<LanguageController>();
    }

    public async Task<IActionResult> EmailConfirm(string userId, string code)
    {
        var user = await _dbContext
            .Users
            .Include(t => t.Emails)
            .SingleOrDefaultAsync(t => t.Id == userId);
        if (user == null)
        {
            return NotFound();
        }

        var mailObject = await _dbContext
            .UserEmails
            .SingleOrDefaultAsync(t => t.ValidateToken == code);

        if (mailObject == null || mailObject.OwnerId != user.Id || mailObject.Validated)
        {
            return BadRequest();
        }

        return View(new EmailConfirmViewModel
        {
            Code = code,
            UserId = userId,
            Mail = mailObject.EmailAddress
        });
    }

    [HttpPost]
    public async Task<IActionResult> EmailConfirmed(EmailConfirmViewModel model)
    {
        var user = await _dbContext
            .Users
            .Include(t => t.Emails)
            .SingleOrDefaultAsync(t => t.Id == model.UserId);
        if (user == null)
        {
            return NotFound();
        }

        var mailObject = await _dbContext
            .UserEmails
            .SingleOrDefaultAsync(t => t.ValidateToken == model.Code);

        if (mailObject == null || mailObject.OwnerId != user.Id)
        {
            return NotFound();
        }

        if (!mailObject.Validated)
        {
            _logger.LogWarning(
                "The email object with address: {EmailAddress} was already validated but the user was still trying to validate it!", mailObject.EmailAddress);
        }

        mailObject.Validated = true;
        mailObject.ValidateToken = string.Empty;
        await _dbContext.SaveChangesAsync();
        return View();
    }

    [HttpGet]
    public IActionResult ForgotPasswordFor()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPasswordFor(ForgotPasswordForViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var mail = await _dbContext.UserEmails.SingleOrDefaultAsync(t => t.EmailAddress == model.Email.ToLower());
        if (mail != null)
        {
            return RedirectToAction(nameof(MethodSelection), new { id = mail.OwnerId });
        }

        ModelState.AddModelError(nameof(model.Email), $"The account with Email: {model.Email} was not found!");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> MethodSelection(string id) //User id
    {
        var user = await _dbContext
            .Users
            .Include(t => t.Emails)
            .SingleOrDefaultAsync(t => t.Id == id);

        if (user == null)
        {
            return NotFound();
        }

        var model = new MethodSelectionViewModel
        {
            AccountName = user.Email,
            SmsResetAvailable = user.PhoneNumberConfirmed,
            PhoneNumber = user.PhoneNumber?[^4..] ?? string.Empty,
            AvailableEmails = user.Emails.Where(t => t.Validated)
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPasswordViaEmail(ForgotPasswordViaEmailViewModel model)
    {
        var mail = await _dbContext.UserEmails.SingleOrDefaultAsync(t => t.EmailAddress == model.Email.ToLower());
        if (mail == null)
        {
            return NotFound();
        }

        var user = await _dbContext
            .Users
            .Include(t => t.Emails)
            .SingleOrDefaultAsync(t => t.Id == mail.OwnerId);
        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        // limit the sending frequency to 3 minutes.
        if (DateTime.UtcNow <= mail.LastSendTime + new TimeSpan(0, 1, 0))
        {
            return RedirectToAction(nameof(ForgotPasswordSent));
        }

        mail.LastSendTime = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
        _cannonService.FireAsync<ConfirmationEmailSender>(async sender =>
        {
            await sender.SendResetPassword(code, user.Id, mail.EmailAddress);
        });
        return RedirectToAction(nameof(ForgotPasswordSent));
    }

    [HttpGet]
    public IActionResult ForgotPasswordSent()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPasswordViaSms(ForgotPasswordViaEmailViewModel model)
    {
        var mail = await _dbContext.UserEmails.SingleOrDefaultAsync(t => t.EmailAddress == model.Email.ToLower());
        if (mail == null)
        {
            return NotFound();
        }

        var user = await _dbContext
            .Users
            .Include(t => t.Emails)
            .SingleOrDefaultAsync(t => t.Id == mail.OwnerId);
        if (user.PhoneNumberConfirmed == false)
        {
            return NotFound();
        }

        var code = StringExtends.RandomString(6);
        user.SMSPasswordResetToken = code;
        await _userManager.UpdateAsync(user);
        _cannonService.FireAsync<APISMSSender>(async sender =>
        {
            await sender.SendAsync(user.PhoneNumber, code + " is your Aiursoft password reset code.");
        });
        return RedirectToAction(nameof(EnterSmsCode), new { model.Email });
    }

    public async Task<IActionResult> EnterSmsCode(string email)
    {
        var mail = await _dbContext.UserEmails.SingleOrDefaultAsync(t => t.EmailAddress == email.ToLower());
        if (mail == null)
        {
            return NotFound();
        }

        var user = await _dbContext
            .Users
            .Include(t => t.Emails)
            .SingleOrDefaultAsync(t => t.Id == mail.OwnerId);
        if (user == null || user.PhoneNumberConfirmed == false)
        {
            return NotFound();
        }

        var phoneLast = user.PhoneNumber.Substring(user.PhoneNumber.Length - 4);
        var model = new EnterSMSCodeViewModel
        {
            Email = email,
            PhoneLast = phoneLast
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnterSmsCode(EnterSMSCodeViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var mail = await _dbContext.UserEmails.SingleOrDefaultAsync(t => t.EmailAddress == model.Email.ToLower());
        if (mail == null)
        {
            return NotFound();
        }

        var user = await _dbContext
            .Users
            .Include(t => t.Emails)
            .SingleOrDefaultAsync(t => t.Id == mail.OwnerId);
        if (user.SMSPasswordResetToken.ToLower().Trim() == model.Code.ToLower().Trim())
        {
            user.SMSPasswordResetToken = string.Empty;
            await _userManager.UpdateAsync(user);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return RedirectToAction(nameof(ResetPassword), new { code = token });
        }

        ModelState.AddModelError("", "Your code is not correct and we can't help you reset your password!");
        return View(model);
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }

    #region Reset password

    [HttpGet]
    public IActionResult ResetPassword(string code = null)
    {
        if (code == null)
        {
            return RedirectToAction(nameof(ForgotPasswordFor));
        }

        var model = new ResetPasswordViewModel
        {
            Code = code
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // What if the user deleted all his emails?
        var mail = await _dbContext.UserEmails.SingleOrDefaultAsync(t => t.EmailAddress == model.Email.ToLower());
        if (mail == null)
        {
            return NotFound();
        }

        var user = await _dbContext
            .Users
            .Include(t => t.Emails)
            .SingleOrDefaultAsync(t => t.Id == mail.OwnerId);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, $"Can not find target user with email '{model.Email}'.");
            return View();
        }

        var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
        if (result.Succeeded)
        {
            return RedirectToAction(nameof(Confirmation));
        }

        AddErrors(result);
        return View();
    }

    [HttpGet]
    public IActionResult Confirmation()
    {
        return View();
    }

    #endregion
}