using System;
namespace ClinicService.IdentityServer.ViewModels
{
    public class ReappointmentRequestModel
    {
        public DateTime ReappointmentDate { get; set; }

        public string Note { get; set; }

        public string PatientId { get; set; }

        public string StatusCategoryId { get; set; }

        public int FromMedicalExaminationId { get; set; }
    }
}
