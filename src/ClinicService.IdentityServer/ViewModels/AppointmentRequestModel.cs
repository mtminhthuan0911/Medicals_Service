using System;
namespace ClinicService.IdentityServer.ViewModels
{
    public class AppointmentRequestModel
    {
        public string AppointmentDate { get; set; }

        public string Note { get; set; }

        public int? MedicalServiceId { get; set; }

        public string GuessFullName { get; set; }

        public string GuessPhoneNumber { get; set; }

        public string PatientId { get; set; }

        public string StatusCategoryId { get; set; }

        public int ClinicBranchId { get; set; }
    }
}
