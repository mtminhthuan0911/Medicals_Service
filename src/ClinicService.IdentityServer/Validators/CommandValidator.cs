using System;
using ClinicService.IdentityServer.Constants;
using ClinicService.IdentityServer.ViewModels;
using FluentValidation;

namespace ClinicService.IdentityServer.Validators
{
    public class CommandValidator : AbstractValidator<CommandRequestModel>
    {
        public CommandValidator()
        {
            RuleFor(r => r.Id)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Id"));

            RuleFor(r => r.Name)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Name"))
                .MaximumLength(256).WithMessage(string.Format(MessagesConstant.RECORD_MAX_LENGTH, "Name", 256));
        }
    }
}
