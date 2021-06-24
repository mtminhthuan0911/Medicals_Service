using System;
namespace ClinicService.IdentityServer.ViewModels
{
    public class PaymentViewModel
    {
        public int AppointmentId { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string PaymentMethodId { get; set; }

        public string PaymentMethodName { get; set; }

        public string StatusCategoryId { get; set; }

        public string StatusCategoryName { get; set; }
    }
}
