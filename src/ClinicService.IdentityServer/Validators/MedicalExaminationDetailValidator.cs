using System;
using ClinicService.IdentityServer.Constants;
using ClinicService.IdentityServer.ViewModels;
using FluentValidation;

namespace ClinicService.IdentityServer.Validators
{
    public class MedicalExaminationDetailValidator : AbstractValidator<MedicalExaminationDetailRequestModel>
    {
        public MedicalExaminationDetailValidator()
        {
            RuleFor(r => r.DoctorId)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Doctor Id"));

            RuleFor(r => r.MedicalExaminationId)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Medical Examination Id"));
        }
    }
}
