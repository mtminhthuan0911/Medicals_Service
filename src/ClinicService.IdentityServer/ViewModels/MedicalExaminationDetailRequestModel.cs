using System;
namespace ClinicService.IdentityServer.ViewModels
{
    public class MedicalExaminationDetailRequestModel
    {
        public string Diagnostic { get; set; }

        public string Treatment { get; set; }

        public string DoctorId { get; set; }

        public int MedicalExaminationId { get; set; }
    }
}
