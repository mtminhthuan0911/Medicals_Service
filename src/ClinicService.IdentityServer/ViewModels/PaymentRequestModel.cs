using System;
namespace ClinicService.IdentityServer.ViewModels
{
    public class PaymentRequestModel
    {
        public int AppointmentId { get; set; }

        public string PaymentMethodId { get; set; }

        public string StatusCategoryId { get; set; }
    }
}
