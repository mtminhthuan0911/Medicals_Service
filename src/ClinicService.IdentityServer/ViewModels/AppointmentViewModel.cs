using System;
namespace ClinicService.IdentityServer.ViewModels
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string Note { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? MedicalServiceId { get; set; }

        public string MedicalServiceTitle { get; set; }

        public string PatientId { get; set; }

        public string PatientFullName { get; set; }

        public string StatusCategoryId { get; set; }

        public string StatusCategoryName { get; set; }

        public int ClinicBranchId { get; set; }

        public string ClinicBranchName { get; set; }

        public string PaymentMethodName { get; set; }

        public string PaymentStatusCategoryName { get; set; }

        public string GuessFullName { get; set; }

        public string GuessPhoneNumber { get; set; }
    }
}
