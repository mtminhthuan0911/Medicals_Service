using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace ClinicService.IdentityServer.ViewModels
{
    public class MedicalExaminationRequestModel
    {
        public string PatientId { get; set; }

        public string StatusCategoryId { get; set; }

        public int? AppointmentId { get; set; }

        public int? ReappointmentId { get; set; }

        public IEnumerable<IFormFile> Attachments { get; set; }
    }
}
