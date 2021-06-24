using System;
using ClinicService.IdentityServer.Constants;
using ClinicService.IdentityServer.ViewModels;
using FluentValidation;

namespace ClinicService.IdentityServer.Validators
{
    public class MedicalExaminationValidator : AbstractValidator<MedicalExaminationRequestModel>
    {
        public MedicalExaminationValidator()
        {
            RuleFor(r => r.PatientId)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Patient Id"));

            RuleFor(r => r.StatusCategoryId)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Status Category Id"));
        }
    }
}
