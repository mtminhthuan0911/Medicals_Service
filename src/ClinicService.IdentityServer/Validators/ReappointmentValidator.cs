using System;
using ClinicService.IdentityServer.Constants;
using ClinicService.IdentityServer.ViewModels;
using FluentValidation;

namespace ClinicService.IdentityServer.Validators
{
    public class ReappointmentValidator : AbstractValidator<ReappointmentRequestModel>
    {
        public ReappointmentValidator()
        {
            RuleFor(r => r.ReappointmentDate)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Reappointment Date"));

            RuleFor(r => r.PatientId)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Patient Id"));

            RuleFor(r => r.StatusCategoryId)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Status Category"));
        }
    }
}
