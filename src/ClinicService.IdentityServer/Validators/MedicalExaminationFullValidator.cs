using System;
using ClinicService.IdentityServer.Constants;
using ClinicService.IdentityServer.ViewModels;
using FluentValidation;

namespace ClinicService.IdentityServer.Validators
{
    public class MedicalExaminationFullValidator : AbstractValidator<MedicalExaminationFullRequestModel>
    {
        public MedicalExaminationFullValidator()
        {
            RuleFor(r => r.PatientId)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Patient Id"));

            RuleFor(r => r.StatusCategoryId)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Status Category Id"));

            RuleFor(r => r.DetailRequestModels)
                .NotNull().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Examination Detail"));

            RuleFor(r => r.PrescriptionRequestModels)
                .NotNull().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Prescription"));
        }
    }
}
