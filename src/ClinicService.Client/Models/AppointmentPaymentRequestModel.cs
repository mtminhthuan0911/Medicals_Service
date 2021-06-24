using System;
namespace ClinicService.Client.Models
{
    public class AppointmentPaymentRequestModel
    {
        public DateTime AppointmentDate { get; set; }

        public string Note { get; set; }

        public int? MedicalServiceId { get; set; }

        public string PatientFullName { get; set; }

        public string PatientPhoneNumber { get; set; }

        public string PatientId { get; set; }

        public string StatusCategoryId { get; set; }

        public int ClinicBranchId { get; set; }

        public string PaymentMethodId { get; set; }

        public string PaymentStatusCategoryId { get; set; }
    }
}
