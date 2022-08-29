using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.CustomeIdentity
{
    public class PersianIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError()
            {
                Code = nameof(DuplicateEmail),
                Description = $"ایمیل '{email}' توسط شخص دیگری انتخاب شده است"
            };
        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError()
            {
                Code = nameof(DuplicateUserName),
                Description = $"شما با این شماره همراه قبلا ثبت نام کرده اید"
            };
        }

        public override IdentityError InvalidEmail(string email)
        {
            return new IdentityError()
            {
                Code = nameof(InvalidEmail),
                Description = $"ایمیل '{email}' یک ایمیل معتبر نیست"
            };
        }

        public override IdentityError DuplicateRoleName(string role)
        {
            return new IdentityError()
            {
                Code = nameof(DuplicateRoleName),
                Description = $"نقش '{role}' قبلا ثبت شده است"
            };
        }

        public override IdentityError InvalidRoleName(string role)
        {
            return new IdentityError()
            {
                Code = nameof(InvalidRoleName),
                Description = $"نام '{role}' معتبر نیست"
            };
        }

        public override IdentityError PasswordRequiresDigit()
        {
            return new IdentityError()
            {
                Code = nameof(PasswordRequiresDigit),
                Description = $"رمز عبور باید حداقل دارای یک عدد باشد"
            };
        }

        public override IdentityError PasswordRequiresLower()
        {
            return new IdentityError()
            {
                Code = nameof(PasswordRequiresLower),
                Description = $"رمز عبور باید حداقل دارای یک کاراکتر انگلیسی کوچک باشد ('a'-'z')"
            };
        }

        public override IdentityError PasswordRequiresUpper()
        {
            return new IdentityError()
            {
                Code = nameof(PasswordRequiresUpper),
                Description = $"رمز عبور باید حداقل دارای یک کاراکتر انگلیسی بزرگ باشد ('A'-'Z')"
            };
        }

        public override IdentityError PasswordRequiresNonAlphanumeric()
        {
            return new IdentityError()
            {
                Code = nameof(PasswordRequiresNonAlphanumeric),
                Description = $"رمز عبور باید حداقل دارای یک کاراکتر ویژه باشد مثل '@#%^&'"
            };
        }

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
            => new IdentityError()
            {
                Code = nameof(PasswordRequiresUniqueChars),
                Description = $"رمز عبور باید حداقل دارای {uniqueChars} کاراکتر منحصر به فرد باشد"
            };

        public override IdentityError PasswordTooShort(int length)
            => new IdentityError()
            {
                Code = nameof(PasswordTooShort),
                Description = $"رمز عبور نباید کمتر از {length} کاراکتر باشد"
            };

        public override IdentityError InvalidUserName(string userName)
            => new IdentityError()
            {
                Code = nameof(InvalidUserName),
                Description = $"نام کاربری '{userName}' معتبر نیست"
            };

        public override IdentityError UserNotInRole(string role)
            => new IdentityError()
            {
                Code = nameof(UserNotInRole),
                Description = $"کاربر مورد نظر در نقش '{role}' نیست"
            };

        public override IdentityError UserAlreadyInRole(string role)
            => new IdentityError()
            {
                Code = nameof(UserAlreadyInRole),
                Description = $"کاربر مورد نظر در نقش '{role}' است"
            };

        public override IdentityError DefaultError()
            => new IdentityError()
            {
                Code = nameof(DefaultError),
                Description = "خطای پیشبینی نشده رخ داد"
            };

        public override IdentityError ConcurrencyFailure()
            => new IdentityError()
            {
                Code = nameof(ConcurrencyFailure),
                Description = "خطای همزمانی رخ داد"
            };

        public override IdentityError InvalidToken()
            => new IdentityError()
            {
                Code = nameof(InvalidToken),
                Description = "توکن معتبر نیست"
            };

        public override IdentityError RecoveryCodeRedemptionFailed()
            => new IdentityError()
            {
                Code = nameof(RecoveryCodeRedemptionFailed),
                Description = "کد بازیابی معتبر نیست"
            };

        public override IdentityError UserLockoutNotEnabled()
            => new IdentityError()
            {
                Code = nameof(UserLockoutNotEnabled),
                Description = "قابلیت قفل اکانت کاربر فعال نیست"
            };

        public override IdentityError UserAlreadyHasPassword()
            => new IdentityError()
            {
                Code = nameof(UserAlreadyHasPassword),
                Description = "کاربر از قبل رمزعبور دارد"
            };

        public override IdentityError PasswordMismatch()
            => new IdentityError()
            {
                Code = nameof(PasswordMismatch),
                Description = "عدم تطابق رمزعبور"
            };

        public override IdentityError LoginAlreadyAssociated()
            => new IdentityError()
            {
                Code = nameof(LoginAlreadyAssociated),
                Description = "از قبل اکانت خارجی به حساب این کاربر متصل اصت"
            };
    }
}
