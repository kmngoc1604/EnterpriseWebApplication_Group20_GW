using Microsoft.AspNetCore.Authorization;
using System.Text;
using System.Threading.Tasks;
using EnterpriseWebApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using XTLASPNET;

namespace EnterpriseWebApplication.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        public RegisterConfirmationModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public string Email { get; set; }

        public string UrlContinue { set; get; }


        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }


            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"No user with this email: '{email}'.");
            }

            if (user.EmailConfirmed)
            {
                return ViewComponent(MessagePage.COMPONENTNAME,
                        new MessagePage.Message()
                        {
                            title = "Alert",
                            htmlcontent = "Email confirmed, redirecting",
                            urlredirect = (returnUrl != null) ? returnUrl : Url.Page("/Index")
                        }

                );
            }

            Email = email;

            if (returnUrl != null)
            {
                UrlContinue = Url.Page("RegisterConfirmation", new { email = Email, returnUrl = returnUrl });
            }
            else
                UrlContinue = Url.Page("RegisterConfirmation", new { email = Email });


            return Page();
        }
    }
}
