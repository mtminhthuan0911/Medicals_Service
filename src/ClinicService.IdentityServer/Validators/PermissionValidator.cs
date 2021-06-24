using System;
using ClinicService.IdentityServer.Constants;
using ClinicService.IdentityServer.ViewModels;
using FluentValidation;

namespace ClinicService.IdentityServer.Validators
{
    public class PermissionValidator : AbstractValidator<PermissionRequestModel>
    {
        public PermissionValidator()
        {
            RuleFor(r => r.FunctionId)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Function Id"));

            RuleFor(r => r.CommandId)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Command Id"));

            RuleFor(r => r.RoleId)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Role Id"));
        }
    }
}
