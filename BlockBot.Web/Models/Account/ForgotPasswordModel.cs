using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace BlockBot.Web.Models.Account
{
    public class ForgotPasswordModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }
    }
}
