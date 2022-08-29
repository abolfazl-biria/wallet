using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.CustomeIdentity
{
    public class ResetPasswordTokenProvider : TotpSecurityStampBasedTokenProvider<MyUser>
    {
        public const string ProviderKey = "ResetPassword";

        public override Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<MyUser> manager, MyUser user)
        {
            return Task.FromResult(false);
        }
    }
}
