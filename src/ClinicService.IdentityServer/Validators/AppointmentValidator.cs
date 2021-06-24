using System;
using ClinicService.IdentityServer.Constants;
using ClinicService.IdentityServer.ViewModels;
using FluentValidation;

namespace ClinicService.IdentityServer.Validators
{
    public class AppointmentValidator : AbstractValidator<AppointmentRequestModel>
    {
        public AppointmentValidator()
        {
            RuleFor(r => r.PatientId)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Patient Id"));

            RuleFor(r => r.StatusCategoryId)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Status Category Id"));

            RuleFor(r => r.ClinicBranchId)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Clinic Branch Id"));
        }
    }
}
