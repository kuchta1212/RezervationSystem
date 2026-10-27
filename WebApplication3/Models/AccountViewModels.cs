using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ReservationSystem.Utils;

namespace ReservationSystem.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }

        [Display(Name = "Pamatuj si mě?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        // Czech/Slovak national number (9 digits, optional separators, optional Slovak-style leading 0) or an
        // international number starting with + or 00. Bare 10-11 digit numbers - what registration bots
        // typed - are refused.
        public const string PhonePattern = @"^\s*(?:\(?(?:\+|00)[\s\-.()]*\d(?:[\s\-.()]*\d){7,14}|0?[2-9](?:[\s\-.]*\d){8})\s*$";

        [Required]
        [StringLength(80, ErrorMessage = "{0} může mít nejvýše {1} znaků.")]
        [NoLinks(ErrorMessage = "{0} nesmí obsahovat odkazy ani e-mailové adresy.")]
        [Display(Name = "Jméno")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Telefon je povinný")]
        [RegularExpression(PhonePattern, ErrorMessage = "Zadejte telefon ve tvaru 777 123 456 nebo +420 777 123 456.")]
        [Display(Name = "Telefon")]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100, ErrorMessage = "{0} může mít nejvýše {1} znaků.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} musí mát alespoň {2} znaků.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potvrdit heslo")]
        [Compare("Password", ErrorMessage = "Hesla nejsou shodná")]
        public string ConfirmPassword { get; set; }

        // Bot protection (see RegistrationGuard): a honeypot that people never see and a signed
        // "form rendered at" token. Neither is ever filled in by the user.
        public string Website { get; set; }

        public string FormToken { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} musí mát alespoň {2} znaků.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potvrzení heslo")]
        [Compare("Password", ErrorMessage = "Hesla nejsou shodná")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
