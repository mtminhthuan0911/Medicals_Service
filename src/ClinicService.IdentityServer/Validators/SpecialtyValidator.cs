using System;
using ClinicService.IdentityServer.Constants;
using ClinicService.IdentityServer.ViewModels;
using FluentValidation;

namespace ClinicService.IdentityServer.Validators
{
    public class SpecialtyValidator : AbstractValidator<SpecialtyRequestModel>
    {
        public SpecialtyValidator()
        {
            RuleFor(r => r.Id)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Id"));

            RuleFor(r => r.Name)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Name"))
                .MaximumLength(128).WithMessage(string.Format(MessagesConstant.RECORD_MAX_LENGTH, "Name", 128));

            RuleFor(r => r.Description)
                .MaximumLength(512).WithMessage(string.Format(MessagesConstant.RECORD_MAX_LENGTH, "Description", 512));
        }
    }
}
