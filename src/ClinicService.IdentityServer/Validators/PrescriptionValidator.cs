using System;
using ClinicService.IdentityServer.Constants;
using ClinicService.IdentityServer.ViewModels;
using FluentValidation;

namespace ClinicService.IdentityServer.Validators
{
    public class PrescriptionValidator : AbstractValidator<PrescriptionRequestModel>
    {
        public PrescriptionValidator()
        {
            RuleFor(r => r.Name)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Name"))
                .MaximumLength(256).WithMessage(string.Format(MessagesConstant.RECORD_MAX_LENGTH, "Name", 256));

            RuleFor(r => r.Quantity)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Quantity"))
                .MaximumLength(64).WithMessage(string.Format(MessagesConstant.RECORD_MAX_LENGTH, "Quantity", 64));

            RuleFor(r => r.AvailableQuantity)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Available Quantity"))
                .MaximumLength(64).WithMessage(string.Format(MessagesConstant.RECORD_MAX_LENGTH, "Available Quantity", 64));

            RuleFor(r => r.Total)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Total"));

            RuleFor(r => r.Take)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Take"));

            RuleFor(r => r.DoctorId)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Patient Id"));

            RuleFor(r => r.MedicalExaminationId)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Medical Examination Id"));
        }
    }
}
