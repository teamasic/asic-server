using AsicServer.Core.Constant;
using AsicServer.Core.Entities;
using AsicServer.Core.ViewModels;
using DataService.Repository;
using FirebaseAdmin.Auth;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataService.Validation
{
    public class UserValidation : AbstractValidator<User>
    {


    }
    public class UserAuthenticationValidation : AbstractValidator<UserAuthentication>
    {
        public UserAuthenticationValidation()
        {
            RuleFor(ua => ua.Username)
                .NotEmpty().WithMessage(ErrorMessage.USERNAME_EMPTY);

            RuleFor(ua => ua.Password)
                .NotEmpty().WithMessage(ErrorMessage.PASSWORD_EMPTY);
        }

    }


    public class RegisteredUserValidation : AbstractValidator<RegisteredUser>
    {
        public RegisteredUserValidation(IUserRepository repository)
        {
            RuleFor(u => u.Username)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty().WithMessage(ErrorMessage.USERNAME_EMPTY)
                .Must(username => !repository.IsExisted(username)).WithMessage(ErrorMessage.USERNAME_EXISTED);

            RuleFor(u => u.Password)
                .NotEmpty().WithMessage(ErrorMessage.PASSWORD_EMPTY)
                //.MinimumLength(Constants.MIN_PASSWORD_LENGTH).MaximumLength(Constants.MAX_PASSWORD_LENGTH)
                .WithMessage(ErrorMessage.PASSWORD_LENGTH_NOT_VALID);

            //RuleFor(u => u.Birthdate)
            //    .Must(bd =>
            //    {
            //        if (bd != null)
            //            return (DateTime.Now.Year - bd.GetValueOrDefault().Year) < 100;
            //        return true;
            //    }).WithMessage(ErrorMessage.BIRTHDATE_NOT_VALID);

            //RuleFor(u => u.Email)
            //    .EmailAddress().WithMessage(ErrorMessage.EMAIL_WRONG_FORMAT);

            //RuleFor(u => u.PhoneNumber)
            //    .Matches("[0-9]{10,11}").WithMessage(ErrorMessage.PHONE_WRONG_FORMAT);

            //RuleFor(u => u.Role)
            //    .NotEmpty().WithMessage(ErrorMessage.ROLES_EMPTY)
            //    .Must(r =>
            //    {
            //        var roles = r.Trim().Split(",");
            //        foreach (var role in roles)
            //        {
            //            object roleEnum = null;
            //            Enum.TryParse(typeof(RolesEnum), role, true, out roleEnum);
            //            if (roleEnum == null) return false;
            //        }
            //        return true;
            //    }).WithMessage(ErrorMessage.ROLES_NOT_EXISTED);
        }
    }

    public class FirebaseRegisterExternalValidation : AbstractValidator<FirebaseRegisterExternal>
    {
        public FirebaseAdmin.Auth.FirebaseToken ParsedToken { get; set; }

        public FirebaseRegisterExternalValidation()
        {
            RuleFor(f => f.FirebaseToken)
                .NotEmpty().WithMessage("Cannot empty")
                .MustAsync(ParseToken).WithMessage("Token is not valid");
        }

        private async Task<bool> ParseToken(string fToken, CancellationToken cancellation)
        {
            try
            {
                this.ParsedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(fToken);
                return true;
            }catch(Exception ex)
            {
                return false;
            }
        }


    }

}
