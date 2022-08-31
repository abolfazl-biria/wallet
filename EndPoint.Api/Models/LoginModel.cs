using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace EndPoint.Api.Models
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class LoginModelValidator : AbstractValidator<LoginModel>
    {
        public LoginModelValidator()
        {
            RuleFor(p => p.Username).NotNull().NotEmpty();

            RuleFor(p => p.Password)
                .NotNull()
                .NotEmpty()
                .MinimumLength(8);
        }
    }
}
