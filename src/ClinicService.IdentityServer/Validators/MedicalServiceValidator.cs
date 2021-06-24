using System;
using ClinicService.IdentityServer.Constants;
using ClinicService.IdentityServer.ViewModels;
using FluentValidation;

namespace ClinicService.IdentityServer.Validators
{
    public class MedicalServiceValidator : AbstractValidator<MedicalServiceRequestModel>
    {
        public MedicalServiceValidator()
        {
            RuleFor(r => r.Title)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Title"))
                .MaximumLength(256).WithMessage(string.Format(MessagesConstant.RECORD_MAX_LENGTH, "Title", 256));
        }
    }
}
