using System;
using ClinicService.IdentityServer.Constants;
using ClinicService.IdentityServer.ViewModels;
using FluentValidation;

namespace ClinicService.IdentityServer.Validators
{
    public class PaymentValidator : AbstractValidator<PaymentRequestModel>
    {
        public PaymentValidator()
        {
            RuleFor(r => r.AppointmentId)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Appointment Id"));

            RuleFor(r => r.PaymentMethodId)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Payment Method Id"));

            RuleFor(r => r.StatusCategoryId)
                .NotEmpty().WithMessage(string.Format(MessagesConstant.RECORD_REQUIRED, "Status Category Id"));
        }
    }
}
