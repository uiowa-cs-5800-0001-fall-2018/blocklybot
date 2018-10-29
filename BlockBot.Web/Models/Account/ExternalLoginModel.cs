using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace BlockBot.Web.Models.Account
{
    public class ExternalLoginModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public string LoginProvider { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }
    }
}
