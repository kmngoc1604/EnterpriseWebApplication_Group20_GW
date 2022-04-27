using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using EnterpriseWebApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using XTLASPNET;

namespace EnterpriseWebApplication.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public ConfirmEmailModel(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code, string returnUrl)
        {

            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }


            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"User not found - '{userId}'.");
            }

            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, false);

                return ViewComponent(MessagePage.COMPONENTNAME,
                    new MessagePage.Message()
                    {
                        title = "Alert",
                        htmlcontent = "Email confirmed, redirecting...",
                        urlredirect = (returnUrl != null) ? returnUrl : Url.Page("/Index")
                    }
                );
            }
            else
            {
                StatusMessage = "Email confirmation error";
            }
            return Page();
        }
    }
}
