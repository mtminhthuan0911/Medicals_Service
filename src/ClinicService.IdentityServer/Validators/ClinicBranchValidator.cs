using System;
using ClinicService.IdentityServer.Constants;
using ClinicService.IdentityServer.ViewModels;
using FluentValidation;

namespace ClinicService.IdentityServer.Validators
{
    public class ClinicBranchValidator : AbstractValidator<ClinicBranchRequestModel>
    {
        public ClinicBranchValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Name"));

            RuleFor(r => r.Address)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Address"));

            RuleFor(r => r.PhoneNumber)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Phone Number"));
        }
    }
}
