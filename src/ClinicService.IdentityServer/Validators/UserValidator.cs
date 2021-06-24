using System;
using ClinicService.IdentityServer.Constants;
using ClinicService.IdentityServer.ViewModels;
using FluentValidation;

namespace ClinicService.IdentityServer.Validators
{
    public class UserValidator : AbstractValidator<UserRequestModel>
    {
        public UserValidator()
        {
            RuleFor(r => r.FirstName)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "First Name"))
                .MaximumLength(64).WithMessage(string.Format(MessagesConstant.RECORD_MAX_LENGTH, "First Name", 64));

            RuleFor(r => r.LastName)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Last Name"))
                .MaximumLength(64).WithMessage(string.Format(MessagesConstant.RECORD_MAX_LENGTH, "Last Name", 64));
        }
    }
}
